using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Api.Middleware.RateLimit
{
    /// <summary>
    /// Rate limiter dùng chung, chạy được trên nhiều instance/pod vì toàn bộ logic
    /// đếm + kiểm tra + set TTL đều chạy atomic bên trong Redis thông qua Lua script
    /// (Redis xử lý Lua script đơn luồng nên không bị race condition).
    /// </summary>
    public sealed class RedisRateLimiter : IRateLimiter
    {
        private readonly IConnectionMultiplexer _redis;
        private const string KeyPrefix = "ratelimit:";

        // ---- Fixed Window ----
        // KEYS[1] = redis key
        // ARGV[1] = limit
        // ARGV[2] = window (giây)
        // Trả về: { allowed(0/1), remaining, ttl_còn_lại(ms) }
        private static readonly string FixedWindowScript = @"
        local current = redis.call('INCR', KEYS[1])
        if current == 1 then
            redis.call('PEXPIRE', KEYS[1], ARGV[2])
        end
        local ttl = redis.call('PTTL', KEYS[1])
        if current > tonumber(ARGV[1]) then
            return { 0, 0, ttl }
        end
        return { 1, tonumber(ARGV[1]) - current, ttl }
    ";

        // ---- Sliding Window (dùng Sorted Set, member = request id duy nhất, score = timestamp ms) ----
        // KEYS[1] = redis key
        // ARGV[1] = limit
        // ARGV[2] = window (ms)
        // ARGV[3] = now (ms)
        // ARGV[4] = unique member id (vd: guid) để tránh trùng score bị ghi đè
        // Trả về: { allowed(0/1), remaining, oldest_score_hoặc_-1 }
        private static readonly string SlidingWindowScript = @"
        local key = KEYS[1]
        local limit = tonumber(ARGV[1])
        local window = tonumber(ARGV[2])
        local now = tonumber(ARGV[3])
        local member = ARGV[4]
        local windowStart = now - window
 
        redis.call('ZREMRANGEBYSCORE', key, '-inf', windowStart)
        local count = redis.call('ZCARD', key)
 
        if count >= limit then
            local oldest = redis.call('ZRANGE', key, 0, 0, 'WITHSCORES')
            local oldestScore = -1
            if oldest[2] ~= nil then
                oldestScore = tonumber(oldest[2])
            end
            return { 0, 0, oldestScore }
        end
 
        redis.call('ZADD', key, now, member)
        redis.call('PEXPIRE', key, window)
        local remaining = limit - count - 1
        return { 1, remaining, -1 }
    ";

        public RedisRateLimiter(IConnectionMultiplexer redis)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        }

        public async Task<RateLimitResult> CheckAsync(
            string key,
            int limit,
            TimeSpan window,
            RateLimitAlgorithm algorithm = RateLimitAlgorithm.FixedWindow,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key không được rỗng.", nameof(key));
            if (limit <= 0)
                throw new ArgumentOutOfRangeException(nameof(limit), "Limit phải > 0.");
            if (window <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(window), "Window phải > 0.");

            var db = _redis.GetDatabase();
            var redisKey = (RedisKey)(KeyPrefix + key);

            return algorithm switch
            {
                RateLimitAlgorithm.FixedWindow => await CheckFixedWindowAsync(db, redisKey, limit, window),
                RateLimitAlgorithm.SlidingWindow => await CheckSlidingWindowAsync(db, redisKey, limit, window),
                _ => throw new NotSupportedException($"Thuật toán {algorithm} chưa được hỗ trợ.")
            };
        }

        private static async Task<RateLimitResult> CheckFixedWindowAsync(
            IDatabase db, RedisKey key, int limit, TimeSpan window)
        {
            var result = (RedisValue[])await db.ScriptEvaluateAsync(
                FixedWindowScript,
                new[] { key },
                new RedisValue[] { limit, (long)window.TotalMilliseconds });

            bool allowed = (int)result[0] == 1;
            long remaining = (long)result[1];
            long ttlMs = (long)result[2];
            var resetAt = DateTimeOffset.UtcNow.AddMilliseconds(ttlMs < 0 ? window.TotalMilliseconds : ttlMs);

            return new RateLimitResult
            {
                IsAllowed = allowed,
                Remaining = Math.Max(0, remaining),
                Limit = limit,
                RetryAfterSeconds = allowed ? 0 : Math.Max(0, ttlMs) / 1000.0,
                ResetAtUtc = resetAt
            };
        }

        private static async Task<RateLimitResult> CheckSlidingWindowAsync(
            IDatabase db, RedisKey key, int limit, TimeSpan window)
        {
            var now = DateTimeOffset.UtcNow;
            var nowMs = now.ToUnixTimeMilliseconds();
            var member = Guid.NewGuid().ToString("N");

            var result = (RedisValue[])await db.ScriptEvaluateAsync(
                SlidingWindowScript,
                new[] { key },
                new RedisValue[] { limit, (long)window.TotalMilliseconds, nowMs, member });

            bool allowed = (int)result[0] == 1;
            long remaining = (long)result[1];
            long oldestScoreMs = (long)result[2];

            DateTimeOffset resetAt;
            double retryAfterSeconds;
            if (!allowed && oldestScoreMs >= 0)
            {
                // Request cũ nhất trong window sẽ hết hạn khi nào -> đó là lúc có slot trống tiếp theo
                resetAt = DateTimeOffset.FromUnixTimeMilliseconds(oldestScoreMs).Add(window);
                retryAfterSeconds = Math.Max(0, (resetAt - now).TotalSeconds);
            }
            else
            {
                resetAt = now.Add(window);
                retryAfterSeconds = 0;
            }

            return new RateLimitResult
            {
                IsAllowed = allowed,
                Remaining = Math.Max(0, remaining),
                Limit = limit,
                RetryAfterSeconds = retryAfterSeconds,
                ResetAtUtc = resetAt
            };
        }
    }
}

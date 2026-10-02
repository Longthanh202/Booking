using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Api.Middleware.RateLimit
{
    /// <summary>
    /// Kết quả trả về sau khi kiểm tra rate limit.
    /// </summary>
    public sealed record RateLimitResult
    {
        /// <summary>Request có được phép đi tiếp hay không.</summary>
        public bool IsAllowed { get; init; }

        /// <summary>Số request còn lại được phép trong window hiện tại.</summary>
        public long Remaining { get; init; }

        /// <summary>Giới hạn tối đa được cấu hình.</summary>
        public long Limit { get; init; }

        /// <summary>Thời gian (giây) client nên chờ trước khi thử lại (chỉ có ý nghĩa khi bị chặn).</summary>
        public double RetryAfterSeconds { get; init; }

        /// <summary>Thời điểm window hiện tại sẽ reset (UTC).</summary>
        public DateTimeOffset ResetAtUtc { get; init; }
    }

    /// <summary>
    /// Thuật toán rate limiting hỗ trợ.
    /// </summary>
    public enum RateLimitAlgorithm
    {
        /// <summary>Đếm số request trong 1 khung giờ cố định (nhanh, tốn ít bộ nhớ, có thể "rò" ở biên window).</summary>
        FixedWindow,

        /// <summary>Sliding window chính xác hơn, dùng Sorted Set để lưu timestamp từng request.</summary>
        SlidingWindow
    }

    /// <summary>
    /// Interface dùng chung để inject và gọi ở bất kỳ đâu: middleware, controller, filter, background job...
    /// </summary>
    public interface IRateLimiter
    {
        /// <summary>
        /// Kiểm tra và ghi nhận 1 request cho <paramref name="key"/>.
        /// Đây là 1 thao tác atomic ở phía Redis (dùng Lua script) nên an toàn khi nhiều instance/pod cùng gọi.
        /// </summary>
        /// <param name="key">
        /// Khóa định danh đối tượng bị giới hạn, ví dụ: "login:{ip}", "api:{userId}:{endpoint}", "otp:{phone}".
        /// </param>
        /// <param name="limit">Số request tối đa cho phép trong 1 window.</param>
        /// <param name="window">Độ dài của window (ví dụ 1 phút, 1 giờ...).</param>
        /// <param name="algorithm">Thuật toán muốn dùng (mặc định FixedWindow).</param>
        Task<RateLimitResult> CheckAsync(
            string key,
            int limit,
            TimeSpan window,
            RateLimitAlgorithm algorithm = RateLimitAlgorithm.FixedWindow,
            CancellationToken cancellationToken = default);
    }
}

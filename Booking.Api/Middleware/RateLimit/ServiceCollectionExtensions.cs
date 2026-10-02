using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Api.Middleware.RateLimit
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Đăng ký IRateLimiter dùng chung trong toàn bộ ứng dụng.
        /// Gọi trong Program.cs: builder.Services.AddRedisRateLimiter("localhost:6379");
        /// </summary>
        public static IServiceCollection AddRedisRateLimiter(this IServiceCollection services, string redisConnectionString)
        {
            // Dùng chung 1 ConnectionMultiplexer (singleton) cho toàn app, KHÔNG tạo mới mỗi lần gọi
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(redisConnectionString));

            services.AddSingleton<IRateLimiter, RedisRateLimiter>();
            return services;
        }

        /// <summary>
        /// Overload cho trường hợp app đã tự quản lý IConnectionMultiplexer riêng.
        /// </summary>
        public static IServiceCollection AddRedisRateLimiter(this IServiceCollection services)
        {
            services.AddSingleton<IRateLimiter, RedisRateLimiter>();
            return services;
        }
    }
}

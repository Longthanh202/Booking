using Booking.Data.Repository.Banners;
using Booking.Data.Repository.DatPhongs;
using Booking.Data.Repository.Emails;
using Booking.Data.Repository.KhachSans;
using Booking.Data.Repository.LoaiPhongs;
using Booking.Data.Repository.Permissions;
using Booking.Data.Repository.Phongs;
using Booking.Data.Repository.Provinces;
using Booking.Data.Repository.RabbitMQ;
using Booking.Data.Repository.Redis;
using Booking.Data.Repository.RefreshTokens;
using Booking.Data.Repository.RolePermissions;
using Booking.Data.Repository.TienIchs;
using Booking.Data.Repository.Users;
using Booking.Service.Consumers.BookingCheckout;
using Booking.Service.Consumers.SendEmailRegister;
using Booking.Service.Services.Auths;
using Booking.Service.Services.Banners;
using Booking.Service.Services.Cloudinarys;
using Booking.Service.Services.DatPhongs;
using Booking.Service.Services.Emails;
using Booking.Service.Services.HoaHongs;
using Booking.Service.Services.KhachSanImage;
using Booking.Service.Services.KhachSans;
using Booking.Service.Services.LichSuVis;
using Booking.Service.Services.LoaiPhongs;
using Booking.Service.Services.Notifications;
using Booking.Service.Services.Permissions;
using Booking.Service.Services.Phongs;
using Booking.Service.Services.Provinces;
using Booking.Service.Services.RabbitMQ;
using Booking.Service.Services.Redis;
using Booking.Service.Services.RefreshTokens;
using Booking.Service.Services.RolePermissions;
using Booking.Service.Services.Roles;
using Booking.Service.Services.TienIchs;
using Booking.Service.Services.Tokens;
using Booking.Service.Services.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Booking.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDIService(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();
            services.AddScoped<IKhachSanService, KhachSanService>();
            services.AddScoped<ITienIchService, TienIchService>();
            services.AddScoped<ILoaiPhongService, LoaiPhongService>();
            services.AddScoped<IPhongService, PhongService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<CloudinaryService>();
            services.AddScoped<IBannerService, BannerService>();
            services.AddScoped<IKhachSanImageService, KhachSanImageService>();
            services.AddScoped<IDatPhongService, DatPhongService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHoaHongService, HoaHongService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ILichSuViService, LichSuViService>();
            
            var redisEnabled = configuration.GetValue<bool>("Redis:Enabled");
            if (redisEnabled)
            {
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                {
                    var connectionString =
                        configuration.GetConnectionString("Redis");

                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        throw new Exception(
                            "Redis được bật nhưng chưa cấu hình ConnectionStrings:Redis");
                    }

                    return ConnectionMultiplexer.Connect(connectionString);
                });

                services.AddScoped<IRedisService, RedisService>();
            }

            //Consumer
            var rabbitMqEnabled = configuration.GetValue<bool>("RabbitMQ:Enabled");

            if (rabbitMqEnabled)
            {
                services.AddHostedService<EmailConsumer>();
                services.AddHostedService<SendEmailBookingConsumer>();
                services.AddHostedService<BookingCheckoutConsumner>();
            }
            return services;
        }
    }
}

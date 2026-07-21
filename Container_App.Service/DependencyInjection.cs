using Container_App.Data.Connection;
using Container_App.Data.DBContext;
using Container_App.Data.Repository.Banners;
using Container_App.Data.Repository.DatPhongs;
using Container_App.Data.Repository.Emails;
using Container_App.Data.Repository.KhachSans;
using Container_App.Data.Repository.LoaiPhongs;
using Container_App.Data.Repository.Permissions;
using Container_App.Data.Repository.Phongs;
using Container_App.Data.Repository.Provinces;
using Container_App.Data.Repository.RabbitMQ;
using Container_App.Data.Repository.Redis;
using Container_App.Data.Repository.RefreshTokens;
using Container_App.Data.Repository.RolePermissions;
using Container_App.Data.Repository.TienIchs;
using Container_App.Data.Repository.Users;
using Container_App.Service.Consumers.SendEmailRegister;
using Container_App.Service.Services.Banners;
using Container_App.Service.Services.Cloudinarys;
using Container_App.Service.Services.DatPhongs;
using Container_App.Service.Services.Emails;
using Container_App.Service.Services.KhachSanImage;
using Container_App.Service.Services.KhachSans;
using Container_App.Service.Services.LoaiPhongs;
using Container_App.Service.Services.Permissions;
using Container_App.Service.Services.Phongs;
using Container_App.Service.Services.Provinces;
using Container_App.Service.Services.RabbitMQ;
using Container_App.Service.Services.Redis;
using Container_App.Service.Services.RefreshTokens;
using Container_App.Service.Services.RolePermissions;
using Container_App.Service.Services.Roles;
using Container_App.Service.Services.TienIchs;
using Container_App.Service.Services.Tokens;
using Container_App.Service.Services.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service
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
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<ITokenService, TokenService>();


            //Consumer
            services.AddHostedService<EmailConsumer>();
            return services;
        }
    }
}

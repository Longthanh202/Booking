using Container_App.Data.Connection;
using Container_App.Data.DBContext;
using Container_App.Data.Repository.Banners;
using Container_App.Data.Repository.DatPhongs;
using Container_App.Data.Repository.KhachSanImage;
using Container_App.Data.Repository.KhachSans;
using Container_App.Data.Repository.LoaiPhongs;
using Container_App.Data.Repository.Permissions;
using Container_App.Data.Repository.Phongs;
using Container_App.Data.Repository.Provinces;
using Container_App.Data.Repository.RefreshTokens;
using Container_App.Data.Repository.RolePermissions;
using Container_App.Data.Repository.Roles;
using Container_App.Data.Repository.TienIchs;
using Container_App.Data.Repository.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Container_App.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDIData(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBannerRepository, BannerRepository>();
            services.AddScoped<IDatPhongRepository, DatPhongRepository>();
            services.AddScoped<IKhachSanImageRepository, KhachSanImageRepository>();
            services.AddScoped<IKhachSanRepository, KhachSanRepository>();
            services.AddScoped<ILoaiPhongRepository, LoaiPhongRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IProvinceRepository, ProvinceRepository>();
            services.AddScoped<IPhongRepository, PhongRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<ITienIchRepository, TienIchRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}

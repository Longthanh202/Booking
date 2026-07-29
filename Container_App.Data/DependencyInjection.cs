using Container_App.Data.Connection;
using Container_App.Data.DBContext;
using Container_App.Data.Repository.Auths;
using Container_App.Data.Repository.Banners;
using Container_App.Data.Repository.ChiTietChiTraKhachSans;
using Container_App.Data.Repository.ChiTietHoaDonHoaHongs;
using Container_App.Data.Repository.ChiTraKhachSans;
using Container_App.Data.Repository.DatPhongs;
using Container_App.Data.Repository.GiaPhongs;
using Container_App.Data.Repository.HoaDonHoaHongs;
using Container_App.Data.Repository.HoaHongs;
using Container_App.Data.Repository.KhachSanImage;
using Container_App.Data.Repository.KhachSans;
using Container_App.Data.Repository.KhachSanTienIch;
using Container_App.Data.Repository.LichSuVis;
using Container_App.Data.Repository.LoaiPhongs;
using Container_App.Data.Repository.Permissions;
using Container_App.Data.Repository.PhongDats;
using Container_App.Data.Repository.Phongs;
using Container_App.Data.Repository.Provinces;
using Container_App.Data.Repository.RefreshTokens;
using Container_App.Data.Repository.RolePermissions;
using Container_App.Data.Repository.Roles;
using Container_App.Data.Repository.ThanhToans;
using Container_App.Data.Repository.TienIchs;
using Container_App.Data.Repository.Users;
using Container_App.Data.Repository.ViKhachSans;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            services.AddScoped<IKhachSanTienIchRepository, KhachSanTienIchRepository>();
            services.AddScoped<IPhongDatRepository, PhongDatRepository>();
            services.AddScoped<IThanhToanRepository, ThanhToanRepository>();
            services.AddScoped<IGiaPhongRepository, GiaPhongRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IHoaHongRepository, HoaHongRepository>();
            services.AddScoped<IViKhachSanRepository, ViKhachSanRepository>();
            services.AddScoped<ILichSuViRepository, LichSuViRepository>();
            services.AddScoped<IChiTraKhachSanRepository, ChiTraKhachSanRepository>();
            services.AddScoped<IChiTietChiTraKhachSanRepository, ChiTietChiTraKhachSanRepository>();
            services.AddScoped<IHoaDonHoaHongRepository, HoaDonHoaHongRepository>();
            services.AddScoped<IChiTietHoaDonHoaHongRepository, ChiTietHoaDonHoaHongRepository>();
            return services;
        }
    }
}

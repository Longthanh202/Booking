using Booking.Data.Connection;
using Booking.Data.DBContext;
using Booking.Data.Repository.Auths;
using Booking.Data.Repository.Banners;
using Booking.Data.Repository.ChiTietChiTraKhachSans;
using Booking.Data.Repository.ChiTietHoaDonHoaHongs;
using Booking.Data.Repository.ChiTraKhachSans;
using Booking.Data.Repository.DatPhongs;
using Booking.Data.Repository.GiaPhongs;
using Booking.Data.Repository.HoaDonHoaHongs;
using Booking.Data.Repository.HoaHongs;
using Booking.Data.Repository.KhachSanImage;
using Booking.Data.Repository.KhachSans;
using Booking.Data.Repository.KhachSanTienIch;
using Booking.Data.Repository.LichSuVis;
using Booking.Data.Repository.LoaiPhongs;
using Booking.Data.Repository.Permissions;
using Booking.Data.Repository.PhongDats;
using Booking.Data.Repository.Phongs;
using Booking.Data.Repository.Provinces;
using Booking.Data.Repository.RefreshTokens;
using Booking.Data.Repository.RolePermissions;
using Booking.Data.Repository.Roles;
using Booking.Data.Repository.ThanhToans;
using Booking.Data.Repository.TienIchs;
using Booking.Data.Repository.Users;
using Booking.Data.Repository.ViKhachSans;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Data
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

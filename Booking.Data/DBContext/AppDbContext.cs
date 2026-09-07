using Booking.Core.Model.Banners;
using Booking.Core.Model.ChiTietChiTraKhachSans;
using Booking.Core.Model.ChiTietHoaDonHoaHongs;
using Booking.Core.Model.ChiTraKhachSans;
using Booking.Core.Model.DatPhongs;
using Booking.Core.Model.GiaPhongs;
using Booking.Core.Model.HoaDonHoaHongs;
using Booking.Core.Model.HoaHongs;
using Booking.Core.Model.KhachSanImage;
using Booking.Core.Model.KhachSans;
using Booking.Core.Model.LichSuVis;
using Booking.Core.Model.LoaiPhongs;
using Booking.Core.Model.Permissions;
using Booking.Core.Model.PhongDats;
using Booking.Core.Model.Phongs;
using Booking.Core.Model.Provinces;
using Booking.Core.Model.RefreshTokens;
using Booking.Core.Model.Resources;
using Booking.Core.Model.RolePermissions;
using Booking.Core.Model.Roles;
using Booking.Core.Model.TaiKhoanNganHangs;
using Booking.Core.Model.TienIchs;
using Booking.Core.Model.UserRoles;
using Booking.Core.Model.Users;
using Booking.Core.Model.ViKhachSans;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 1. ÁNH XẠ TÊN BẢNG (Đã lọc bỏ trùng lặp) ---
            modelBuilder.Entity<Banner>().ToTable("Banner");
            modelBuilder.Entity<DatPhong>().ToTable("DatPhong");
            modelBuilder.Entity<KhachSan>().ToTable("KhachSan");
            modelBuilder.Entity<LoaiPhong>().ToTable("LoaiPhong");
            modelBuilder.Entity<Phong>().ToTable("Phong");
            modelBuilder.Entity<ChiTietDatPhong>().ToTable("ChiTietDatPhong");
            modelBuilder.Entity<ThanhToan>().ToTable("ThanhToan");
            modelBuilder.Entity<Permission>().ToTable("Permissions"); // Khớp chữ 's' với SQL của bạn
            modelBuilder.Entity<Province>().ToTable("UserProfile"); // Lưu ý check lại tên bảng thực tế dưới DB (provinces hay province)
            modelBuilder.Entity<RefreshToken>().ToTable("RefreshToken");
            modelBuilder.Entity<Resources>().ToTable("Resources");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<RolePermission>().ToTable("RolePermissions"); // Khớp chữ 's' với SQL
            modelBuilder.Entity<TienIch>().ToTable("TienIch");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<UserProfile>().ToTable("UserProfile");
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");
            modelBuilder.Entity<KhachSanImages>().ToTable("KhachSanImages");
            modelBuilder.Entity<KhachSan_TienIch>().ToTable("KhachSan_TienIch");
            modelBuilder.Entity<TaiKhoanNganHang>().ToTable("TaiKhoanNganHang");
            modelBuilder.Entity<GiaPhong>().ToTable("GiaPhong");
            modelBuilder.Entity<PhongDat>().ToTable("PhongDat");
            modelBuilder.Entity<ExternalLogin>().ToTable("ExternalLogins");
            modelBuilder.Entity<HoaHong>().ToTable("HoaHong");
            modelBuilder.Entity<ViKhachSan>().ToTable("ViKhachSan");
            modelBuilder.Entity<LichSuVi>().ToTable("LichSuVi");
            modelBuilder.Entity<ChiTraKhachSan>().ToTable("ChiTraKhachSan");
            modelBuilder.Entity<ChiTietChiTraKhachSan>().ToTable("ChiTietChiTraKhachSan");
            modelBuilder.Entity<HoaDonHoaHong>().ToTable("HoaDonHoaHong");
            modelBuilder.Entity<ChiTietHoaDonHoaHong>().ToTable("ChiTietHoaDonHoaHong");

            // Nếu muốn dùng bảng tỉnh thành, hãy đồng nhất 1 dòng ToTable duy nhất:
            modelBuilder.Entity<Province>().ToTable("provinces");


            // --- 2. CẤU HÌNH KHÓA CHÍNH RIÊNG BIỆT TRƯỚC ---
            modelBuilder.Entity<Province>().HasKey(p => p.code);

            // SỬA LỖI: UserRole dùng Id làm khóa chính theo cấu trúc bảng SQL của bạn
            modelBuilder.Entity<UserRole>().HasKey(x => x.Id);


            // --- 3. CẤU HÌNH MỐI QUAN HỆ (RELATIONSHIPS) ---

            // Quan hệ Nhiều - Nhiều: KhachSan_TienIch
            modelBuilder.Entity<KhachSan_TienIch>(entity =>
            {
                entity.HasKey(x => new { x.KhachSanId, x.TienIchId });

                entity.HasOne(x => x.KhachSan)
                    .WithMany(x => x.KhachSan_TienIches)
                    .HasForeignKey(x => x.KhachSanId);

                entity.HasOne(x => x.TienIch)
                    .WithMany(x => x.KhachSan_TienIches)
                    .HasForeignKey(x => x.TienIchId);
            });

            // Quan hệ 1 - Nhiều: KhachSan -> LoaiPhong
            modelBuilder.Entity<LoaiPhong>(entity =>
            {
                entity.HasOne(x => x.KhachSan)
                    .WithMany(x => x.LoaiPhongs)
                    .HasForeignKey(x => x.KhachSanId);
            });

            // Quan hệ 1 - Nhiều: KhachSan -> KhachSanImages
            modelBuilder.Entity<KhachSanImages>(entity =>
            {
                entity.HasOne(x => x.KhachSan)
                    .WithMany(x => x.KhachSanImages)
                    .HasForeignKey(x => x.KhachSanId);
            });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.UserLogin)
                .WithOne(ul => ul.UserRole)            // UserLogin chỉ có 1 UserRole duy nhất
                .HasForeignKey<UserRole>(ur => ur.UserId); // Khóa ngoại đặt tại bảng UserRole (cột UserId)

            modelBuilder.Entity<UserProfile>()
                .HasOne(p => p.UserRole)
                .WithOne(r => r.UserProfile)
                .HasForeignKey<UserRole>(r => r.Id);

            // Liên kết KhachSan -> Province qua trường ngoại lai ThanhPho mapping với code của Province
            modelBuilder.Entity<KhachSan>()
                .HasOne(x => x.Province)
                .WithMany()
                .HasForeignKey(x => x.ThanhPho)
                .HasPrincipalKey(x => x.code);

            // HoaHong
            modelBuilder.Entity<HoaHong>(entity =>
            {            

                entity.HasKey(x => x.MaHoaHong);

                entity.HasOne(x => x.DatPhong)
                    .WithMany()
                    .HasForeignKey(x => x.MaDatPhong)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.KhachSan)
                    .WithMany(x => x.HoaHongs)
                    .HasForeignKey(x => x.MaKhachSan)
                    .OnDelete(DeleteBehavior.Restrict);
                
            });

            // ViKhachSan
            modelBuilder.Entity<ViKhachSan>(entity =>
            {              
                entity.HasKey(x => x.MaVi);

                entity.HasOne(x => x.KhachSan)
                    .WithOne(x => x.ViKhachSan)
                    .HasForeignKey<ViKhachSan>(x => x.MaKhachSan)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // LichSuVi
            modelBuilder.Entity<LichSuVi>(entity =>
            {
                entity.HasKey(x => x.MaLichSu);

                entity.HasOne(x => x.ViKhachSan)
                    .WithMany(x => x.LichSuVis)
                    .HasForeignKey(x => x.MaVi)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.DatPhong)
                    .WithMany()
                    .HasForeignKey(x => x.MaDatPhong)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ChiTraKhachSan
            modelBuilder.Entity<ChiTraKhachSan>(entity =>
            {               
                entity.HasKey(x => x.MaChiTra);

                entity.HasOne(x => x.KhachSan)
                    .WithMany(x => x.ChiTraKhachSans)
                    .HasForeignKey(x => x.KhachSanId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.TaiKhoanNganHang)
                    .WithMany()
                    .HasForeignKey(x => x.TaiKhoanNganHangId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ChiTietChiTraKhachSan
            modelBuilder.Entity<ChiTietChiTraKhachSan>(entity =>
            {             
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.ChiTraKhachSan)
                    .WithMany(x => x.ChiTietChiTraKhachSans)
                    .HasForeignKey(x => x.ChiTraKhachSanId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.DatPhong)
                    .WithMany()
                    .HasForeignKey(x => x.DatPhongId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // HoaDonHoaHong
            modelBuilder.Entity<HoaDonHoaHong>(entity =>
            {           
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.KhachSan)
                    .WithMany(x => x.HoaDonHoaHongs)
                    .HasForeignKey(x => x.KhachSanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ChiTietHoaDonHoaHong
            modelBuilder.Entity<ChiTietHoaDonHoaHong>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.HoaDonHoaHong)
                    .WithMany(x => x.ChiTietHoaDonHoaHongs)
                    .HasForeignKey(x => x.HoaDonHoaHongId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.HoaHong)
                    .WithMany(x => x.ChiTietHoaDonHoaHongs)
                    .HasForeignKey(x => x.HoaHongId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

        }

        // --- 4. ĐĂNG KÝ CÁC DBSET ---
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<UserLogin> UserLogins { get; set; } = null!;
        public DbSet<Banner> Banners { get; set; } = null!;
        public DbSet<DatPhong> DatPhongs { get; set; } = null!;
        public DbSet<KhachSan> KhachSans { get; set; } = null!;
        public DbSet<LoaiPhong> LoaiPhongs { get; set; } = null!;
        public DbSet<Phong> Phongs { get; set; } = null!;
        public DbSet<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = null!;
        public DbSet<ThanhToan> ThanhToans { get; set; } = null!;
        public DbSet<KhachSanImages> KhachSanImages { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<Province> Provinces { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public DbSet<TienIch> TienIches { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<KhachSan_TienIch> KhachSan_TienIches { get; set; } = null!;
        public DbSet<Resources> Resources { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<TaiKhoanNganHang> TaiKhoanNganHangs { get; set; } = null!;
        public DbSet<GiaPhong> GiaPhongs { get; set; } = null!;
        public DbSet<PhongDat> phongDats { get; set; } = null!;
        public DbSet<ExternalLogin> ExternalLogins { get; set; } = null!;
        public DbSet<HoaHong> HoaHongs { get; set; }

        public DbSet<ViKhachSan> ViKhachSans { get; set; }

        public DbSet<LichSuVi> LichSuVis { get; set; }

        public DbSet<ChiTraKhachSan> ChiTraKhachSans { get; set; }

        public DbSet<ChiTietChiTraKhachSan> ChiTietChiTraKhachSans { get; set; }

        public DbSet<HoaDonHoaHong> HoaDonHoaHongs { get; set; }

        public DbSet<ChiTietHoaDonHoaHong> ChiTietHoaDonHoaHongs { get; set; }
    }
}
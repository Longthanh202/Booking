using Container_App.Core.Model.Banners;
using Container_App.Core.Model.DatPhongs;
using Container_App.Core.Model.KhachSanImage;
using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.Permissions;
using Container_App.Core.Model.Phongs;
using Container_App.Core.Model.Provinces;
using Container_App.Core.Model.RefreshTokens;
using Container_App.Core.Model.Resources;
using Container_App.Core.Model.RolePermissions;
using Container_App.Core.Model.Roles;
using Container_App.Core.Model.TienIchs;
using Container_App.Core.Model.UserRoles;
using Container_App.Core.Model.Users;
using Microsoft.EntityFrameworkCore;

namespace Container_App.Data.DBContext
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
    }
}
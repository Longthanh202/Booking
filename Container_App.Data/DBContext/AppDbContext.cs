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
using Container_App.Core.Model.TienIchs;
using Container_App.Core.Model.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.DBContext
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Banner>().ToTable("Banner");
            modelBuilder.Entity<DatPhong>().ToTable("DatPhong");
            modelBuilder.Entity<KhachSan>().ToTable("KhachSan");
            modelBuilder.Entity<LoaiPhong>().ToTable("LoaiPhong");
            modelBuilder.Entity<Phong>().ToTable("Phong");
            modelBuilder.Entity<KhachSan>().ToTable("KhachSan");
            modelBuilder.Entity<ChiTietDatPhong>().ToTable("ChiTietDatPhong");
            modelBuilder.Entity<ThanhToan>().ToTable("ThanhToan");
            modelBuilder.Entity<Permission>().ToTable("Permission");
            modelBuilder.Entity<Province>().ToTable("province");
            modelBuilder.Entity<RefreshToken>().ToTable("RefreshToken");
            modelBuilder.Entity<Resources>().ToTable("Resources");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<RolePermission>().ToTable("RolePermission");
            modelBuilder.Entity<TienIch>().ToTable("TienIch");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<UserProfile>().ToTable("UserProfile");
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");
            modelBuilder.Entity<KhachSanImages>().ToTable("KhachSanImages");
            modelBuilder.Entity<KhachSan_TienIch>().ToTable("KhachSan_TienIch");
            modelBuilder.Entity<Province>().ToTable("provinces");

            modelBuilder.Entity<KhachSan_TienIch>(entity =>
            {
                // Khóa chính tổng hợp
                entity.HasKey(x => new
                {
                    x.KhachSanId,
                    x.TienIchId
                });

                // Quan hệ với KhachSan
                entity.HasOne(x => x.KhachSan)
                    .WithMany(x => x.KhachSan_TienIches)
                    .HasForeignKey(x => x.KhachSanId);

                // Quan hệ với TienIch
                entity.HasOne(x => x.TienIch)
                    .WithMany(x => x.KhachSan_TienIchs)
                    .HasForeignKey(x => x.TienIchId);
            });

            modelBuilder.Entity<LoaiPhong>(entity =>
            {
                entity.HasOne(x => x.KhachSan)
                    .WithMany(x => x.LoaiPhongs)
                    .HasForeignKey(x => x.KhachSanId);
            });

            modelBuilder.Entity<KhachSanImages>(entity =>
            {
                entity.HasOne(x => x.KhachSan)
                    .WithMany(x => x.KhachSanImages)
                    .HasForeignKey(x => x.KhachSanId);
            });

            modelBuilder.Entity<UserRole>()
                .HasKey(x => x.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.UserLogin)
                .WithOne(x => x.UserRole)
                .HasForeignKey<UserRole>(x => x.UserId);

            modelBuilder.Entity<KhachSan>()
                .HasOne(x => x.Province)
                .WithMany()
                .HasForeignKey(x => x.ThanhPho)
                .HasPrincipalKey(x => x.code);
            modelBuilder.Entity<Province>().HasKey(p => p.code);
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<DatPhong> DatPhongs { get; set; }
        public DbSet<KhachSan> KhachSans { get; set; }
        public DbSet<LoaiPhong> LoaiPhongs { get; set; }
        public DbSet<Phong> Phongs { get; set; }
        public DbSet<ChiTietDatPhong> ChiTietDatPhongs { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public DbSet<KhachSanImages> KhachSanImages { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<TienIch> TienIches { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<KhachSan_TienIch> KhachSan_TienIches { get; set; }
        public DbSet<Resources> Resources { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}

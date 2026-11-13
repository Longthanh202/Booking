using Booking.Core.Model.Banners;
using Booking.Core.Model.ChiTietChiTraKhachSans;
using Booking.Core.Model.ChiTietHoaDonHoaHongs;
using Booking.Core.Model.ChiTraKhachSans;
using Booking.Core.Model.DanhGias;
using Booking.Core.Model.DatPhongs;
using Booking.Core.Model.GiaPhongs;
using Booking.Core.Model.HoaDonHoaHongs;
using Booking.Core.Model.HoaHongs;
using Booking.Core.Model.KhachSanImage;
using Booking.Core.Model.KhachSans;
using Booking.Core.Model.LichSuVis;
using Booking.Core.Model.LoaiPhongs;
using Booking.Core.Model.PhongDats;
using Booking.Core.Model.Phongs;
using Booking.Core.Model.Provinces;
using Booking.Core.Model.QuangCaos;
using Booking.Core.Model.RefreshTokens;
using Booking.Core.Model.Roles;
using Booking.Core.Model.TaiKhoanNganHangs;
using Booking.Core.Model.TienIchs;
using Booking.Core.Model.UserRoles;
using Booking.Core.Model.Users;
using Booking.Core.Model.ViKhachSans;
using Booking.Core.Model.Tags;
using Microsoft.EntityFrameworkCore;
using Booking.Core.Model.GiayToTuyThans;

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
            modelBuilder.Entity<DatPhong>()
                .Property(x => x.MaDatPhong)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<DanhGia>().ToTable("DanhGia");
            modelBuilder.Entity<KhachSan>().ToTable("KhachSan");
            modelBuilder.Entity<LoaiPhong>().ToTable("LoaiPhong");
            modelBuilder.Entity<Phong>().ToTable("Phong");
            modelBuilder.Entity<ChiTietDatPhong>().ToTable("ChiTietDatPhong");
            modelBuilder.Entity<ThanhToan>().ToTable("ThanhToan");
            modelBuilder.Entity<TokenLamMoi>().ToTable("TokenLamMoi");
            modelBuilder.Entity<VaiTro>().ToTable("VaiTro");
            modelBuilder.Entity<TienIch>().ToTable("TienIch");
            modelBuilder.Entity<TaiKhoan>().ToTable("TaiKhoan");
            modelBuilder.Entity<NguoiDung>().ToTable("NguoiDung");
            modelBuilder.Entity<NguoiDung_VaiTro>().ToTable("NguoiDung_VaiTro");
            modelBuilder.Entity<KhachSanAnh>().ToTable("KhachSanAnh");
            modelBuilder.Entity<KhachSan_TienIch>().ToTable("KhachSan_TienIch");
            modelBuilder.Entity<TaiKhoanNganHang>().ToTable("TaiKhoanNganHang");
            modelBuilder.Entity<GiaPhong>().ToTable("GiaPhong");
            modelBuilder.Entity<PhongDat>().ToTable("PhongDat");
            modelBuilder.Entity<DangNhapNgoai>().ToTable("DangNhapNgoai");
            modelBuilder.Entity<HoaHong>().ToTable("HoaHong");
            modelBuilder.Entity<ViKhachSan>().ToTable("ViKhachSan");
            modelBuilder.Entity<LichSuVi>().ToTable("LichSuVi");
            modelBuilder.Entity<ChiTraKhachSan>().ToTable("ChiTraKhachSan");
            modelBuilder.Entity<ChiTietChiTraKhachSan>().ToTable("ChiTietChiTraKhachSan");
            modelBuilder.Entity<HoaDonHoaHong>().ToTable("HoaDonHoaHong");
            modelBuilder.Entity<ChiTietHoaDonHoaHong>().ToTable("ChiTietHoaDonHoaHong");
            modelBuilder.Entity<GoiQuangCao>().ToTable("GoiQuangCao");
            modelBuilder.Entity<KhachSanQuangCao>().ToTable("KhachSanQuangCao");
            modelBuilder.Entity<TheTag>().ToTable("TheTag");
            modelBuilder.Entity<KhachSan_TheTag>().ToTable("KhachSan_TheTag");
            modelBuilder.Entity<KhoaPhong>().ToTable("KhoaPhong");
            modelBuilder.Entity<KhuyenMaiKhachSan>().ToTable("KhuyenMaiKhachSan");
            modelBuilder.Entity<GiayToTuyThan>().ToTable("GiayToTuyThan");

            modelBuilder.Entity<TinhThanh>().ToTable("TinhThanh");


            // --- 2. CẤU HÌNH KHÓA CHÍNH RIÊNG BIỆT TRƯỚC ---
            modelBuilder.Entity<TinhThanh>().HasKey(p => p.code);

            // SỬA LỖI: NguoiDung_VaiTro dùng Id làm khóa chính theo cấu trúc bảng SQL của bạn
            modelBuilder.Entity<NguoiDung_VaiTro>().HasKey(x => x.Id);

            modelBuilder.Entity<NguoiDung>()
                .HasOne(x => x.TaiKhoan)
                .WithOne(x => x.NguoiDung)
                .HasForeignKey<NguoiDung>(x => x.UserLoginId);

            modelBuilder.Entity<NguoiDung_VaiTro>()
                .HasOne(x => x.VaiTro)
                .WithMany(x => x.NguoiDung_VaiTros)
                .HasForeignKey(x => x.RoleId);

            modelBuilder.Entity<DangNhapNgoai>()
                .HasOne(x => x.TaiKhoan)
                .WithMany()
                .HasForeignKey(x => x.UserLoginId);


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

            modelBuilder.Entity<TheTag>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Slug).IsUnique();
            });

            modelBuilder.Entity<KhachSan_TheTag>(entity =>
            {
                entity.HasKey(x => new { x.HotelId, x.TagId });

                entity.HasOne(x => x.Hotel)
                    .WithMany(x => x.KhachSan_TheTags)
                    .HasForeignKey(x => x.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.TheTag)
                    .WithMany(x => x.KhachSan_TheTags)
                    .HasForeignKey(x => x.TagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<KhoaPhong>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => new { x.RoomId, x.StartAt, x.EndAt });
                entity.HasOne(x => x.Room)
                    .WithMany()
                    .HasForeignKey(x => x.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<KhuyenMaiKhachSan>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => new { x.HotelId, x.Code }).IsUnique();
                entity.Property(x => x.DiscountValue).HasColumnType("decimal(18,2)");
                entity.Property(x => x.MinBookingAmount).HasColumnType("decimal(18,2)");
                entity.Property(x => x.MaxDiscountAmount).HasColumnType("decimal(18,2)");
                entity.HasOne(x => x.Hotel)
                    .WithMany()
                    .HasForeignKey(x => x.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Quan hệ 1 - Nhiều: KhachSan -> LoaiPhong
            modelBuilder.Entity<LoaiPhong>(entity =>
            {
                entity.HasOne(x => x.KhachSan)
                    .WithMany(x => x.LoaiPhongs)
                    .HasForeignKey(x => x.KhachSanId);
            });

            // Quan hệ 1 - Nhiều: KhachSan -> KhachSanAnh
            modelBuilder.Entity<KhachSanAnh>(entity =>
            {
                entity.HasOne(x => x.KhachSan)
                    .WithMany(x => x.KhachSanAnhs)
                    .HasForeignKey(x => x.KhachSanId);
            });

                    // Quan hệ 1 - Nhiều: KhachSan -> DanhGia
                    modelBuilder.Entity<DanhGia>(entity =>
                    {
                    entity.HasKey(x => x.Id);
                    entity.Property(x => x.Id).HasColumnName("Id");
                    entity.Property(x => x.KhachSanId).HasColumnName("KhachSanId");
                    entity.Property(x => x.KhachHangId).HasColumnName("KhachHangId");
                    entity.Property(x => x.SoSao).HasColumnName("SoSao");
                    entity.Property(x => x.NoiDung).HasColumnName("NoiDung").HasColumnType("nvarchar(max)");
                    entity.Property(x => x.NgayTao).HasColumnName("NgayTao");

                    entity.HasOne(x => x.KhachSan)
                        .WithMany(x => x.DanhGias)
                        .HasForeignKey(x => x.KhachSanId)
                        .OnDelete(DeleteBehavior.Restrict);
                    });

            modelBuilder.Entity<NguoiDung_VaiTro>()
                .HasOne(ur => ur.TaiKhoan)
                .WithOne(ul => ul.NguoiDung_VaiTro)            // TaiKhoan chỉ có 1 NguoiDung_VaiTro duy nhất
                .HasForeignKey<NguoiDung_VaiTro>(ur => ur.UserId); // Khóa ngoại đặt tại bảng NguoiDung_VaiTro (cột UserId)

            modelBuilder.Entity<NguoiDung>()
                .HasOne(p => p.NguoiDung_VaiTro)
                .WithOne(r => r.NguoiDung)
                .HasForeignKey<NguoiDung_VaiTro>(r => r.Id);

            // Liên kết KhachSan -> TinhThanh qua trường ngoại lai ThanhPho mapping với code của TinhThanh
            modelBuilder.Entity<KhachSan>()
                .HasOne(x => x.TinhThanh)
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

            modelBuilder.Entity<GiayToTuyThan>(e =>
            {
                e.ToTable("GiayToTuyThan");
                e.Property(x => x.LoaiGiayTo).HasConversion<byte>();
                e.Property(x => x.TrangThaiXacMinh).HasConversion<byte>();
                e.Property(x => x.SoGiayToHash).HasColumnType("binary(32)");
                e.Property(x => x.NgayTao).HasDefaultValueSql("SYSUTCDATETIME()");

                e.HasOne(x => x.NguoiDung)
                 .WithMany(u => u.GiayToTuyThans)
                 .HasForeignKey(x => x.UserProfileId);

                e.HasQueryFilter(x => !x.IsDel);   // tự lọc bản ghi đã xóa mềm
            });
        }

        // --- 4. ĐĂNG KÝ CÁC DBSET ---
        public DbSet<NguoiDung> NguoiDungs { get; set; } = null!;
        public DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;
        public DbSet<Banner> Banners { get; set; } = null!;
        public DbSet<DatPhong> DatPhongs { get; set; } = null!;
        public DbSet<DanhGia> DanhGias { get; set; } = null!;
        public DbSet<KhachSan> KhachSans { get; set; } = null!;
        public DbSet<TheTag> TheTags { get; set; } = null!;
        public DbSet<KhachSan_TheTag> KhachSan_TheTags { get; set; } = null!;
        public DbSet<KhoaPhong> KhoaPhongs { get; set; } = null!;
        public DbSet<KhuyenMaiKhachSan> KhuyenMaiKhachSans { get; set; } = null!;
        public DbSet<LoaiPhong> LoaiPhongs { get; set; } = null!;
        public DbSet<Phong> Phongs { get; set; } = null!;
        public DbSet<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = null!;
        public DbSet<ThanhToan> ThanhToans { get; set; } = null!;
        public DbSet<KhachSanAnh> KhachSanAnhs { get; set; } = null!;
        public DbSet<TinhThanh> TinhThanhs { get; set; } = null!;
        public DbSet<TokenLamMoi> TokenLamMois { get; set; } = null!;
        public DbSet<TienIch> TienIches { get; set; } = null!;
        public DbSet<NguoiDung_VaiTro> NguoiDung_VaiTros { get; set; } = null!;
        public DbSet<KhachSan_TienIch> KhachSan_TienIches { get; set; } = null!;
        public DbSet<VaiTro> VaiTros { get; set; } = null!;
        public DbSet<TaiKhoanNganHang> TaiKhoanNganHangs { get; set; } = null!;
        public DbSet<GiaPhong> GiaPhongs { get; set; } = null!;
        public DbSet<PhongDat> phongDats { get; set; } = null!;
        public DbSet<DangNhapNgoai> DangNhapNgoais { get; set; } = null!;
        public DbSet<HoaHong> HoaHongs { get; set; }
        public DbSet<ViKhachSan> ViKhachSans { get; set; }
        public DbSet<LichSuVi> LichSuVis { get; set; }
        public DbSet<ChiTraKhachSan> ChiTraKhachSans { get; set; }
        public DbSet<ChiTietChiTraKhachSan> ChiTietChiTraKhachSans { get; set; }
        public DbSet<HoaDonHoaHong> HoaDonHoaHongs { get; set; }
        public DbSet<ChiTietHoaDonHoaHong> ChiTietHoaDonHoaHongs { get; set; }
        public DbSet<GoiQuangCao> GoiQuangCaos { get; set; }
        public DbSet<KhachSanQuangCao> KhachSanQuangCaos { get; set; }
        public DbSet<GiayToTuyThan> GiayToTuyThans { get; set; }
    }
}
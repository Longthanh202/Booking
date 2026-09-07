using Booking.Common.Shared.Enum.Booking;
using Booking.Core.Model.KhachSans;
using Booking.Core.Model.LoaiPhongs;
using Booking.Core.Model.TienIchs;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Common.Shared;

namespace Booking.Data.Repository.KhachSans
{
    public class KhachSanRepository : IKhachSanRepository
    {
        private readonly AppDbContext _context;
        public KhachSanRepository(AppDbContext context)
        {
            _context = context;
        }
        // Ở tầng KhachSanRepository
        public async Task<KhachSan?> DetailKhachSan(Guid id)
        {
            return await _context.KhachSans
                .AsNoTracking() // Tăng tốc độ đọc, không track để lưu cache thừa
                .AsSplitQuery() // Tách các truy vấn con ra để tránh lỗi Cartesian explosion
                .Include(ks => ks.Province)
                .Include(ks => ks.LoaiPhongs)
                .Include(ks => ks.KhachSanImages)
                .Include(ks => ks.KhachSan_TienIches)
                    .ThenInclude(kst => kst.TienIch) // Đi sâu vào lấy data bảng TienIch
                .FirstOrDefaultAsync(ks => ks.Id == id);
        }

        public async Task<(List<KhachSan> Items, int TotalCount)> FilterHotels(
    string? keyword, 
    string provinceCode, 
    int? soKhach, 
    DateTime? ngayNhanPhong, 
    DateTime? ngayTraPhong, 
    int pageIndex, 
    int pageSize)
{
    try
    {
        // 1. Sử dụng AsNoTracking() để tăng hiệu năng và tránh lỗi tracking state
        IQueryable<KhachSan> query = _context.KhachSans.AsNoTracking();

        // 2. Lọc theo Keyword
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.TenKhachSan.Contains(keyword));
        }

        // 3. Lọc theo Tỉnh/Thành phố
        if (!string.IsNullOrEmpty(provinceCode))
        {
            query = query.Where(x => x.ThanhPho == provinceCode);
        }

        // 4. Lọc theo Số khách
        if (soKhach.HasValue)
        {
            query = query.Where(x => x.LoaiPhongs.Any(lp => lp.SoKhachToiDa >= soKhach.Value));
        }

        // 5. Lọc theo ngày nhận/trả phòng (Kiểm tra phòng trống)
        if (ngayNhanPhong.HasValue && ngayTraPhong.HasValue)
        {
            string trangThaiDaHuy = TrangThaiDatPhong.DA_HUY.ToString();

            query = query.Where(x => x.LoaiPhongs.Any(lp => 
                !lp.ChiTietDatPhongs.Any(ct =>
                    ct.DatPhong.NgayNhanPhong < ngayTraPhong.Value &&
                    ct.DatPhong.NgayTraPhong > ngayNhanPhong.Value &&
                    ct.DatPhong.TrangThai != trangThaiDaHuy)));
        }

        // 6. Đếm tổng số bản ghi
        int totalCount = await query.CountAsync();

        // Nếu không có bản ghi nào thì trả về luôn, tránh chạy câu Query nặng bên dưới
        if (totalCount == 0)
        {
            return (new List<KhachSan>(), 0);
        }

        // 7. Phân trang
        int skip = (pageIndex - 1) * pageSize;

        // Bổ sung Include LoaiPhongs TẠI BƯỚC NÀY thay vì đặt ở đầu query
        // Sử dụng AsSplitQuery() để tránh lỗi Cartesian Explosion khi Skip/Take có Include
        var items = await query
            .Include(x => x.LoaiPhongs)
            .AsSplitQuery() // <--- GIẢI PHÁP QUAN TRỌNG CHO EF CORE
            .OrderByDescending(x => x.NgayTao)
            .Skip(skip < 0 ? 0 : skip)
            .Take(pageSize)
            .ToListAsync();

        return (items ?? new List<KhachSan>(), totalCount);
    }
    catch (Exception ex)
    {
        FileLogger.Log(ex);
        // Ném lại Exception với thông tin rõ ràng hoặc xử lý tùy nhu cầu
        throw new Exception($"Lỗi khi truy vấn danh sách khách sạn: {ex.Message}", ex);
    }
}
    
        public async Task<KhachSan> TaoKhachSan(KhachSan ks)
        {
            ks.Id = Guid.NewGuid();

            await _context.KhachSans.AddAsync(ks);

            return ks;
        }

        public async Task<(List<KhachSan> Items, int TotalCount)> LayDanhSachKhachSanAdmin(
            string? keyword,
            string? thanhPho,
            double viDo,
            double kinhDo,
            int soSao,
            string? trangThai,
            int pageIndex,
            int pageSize)
        {
            return await LayDanhSachKhachSanInternal(keyword, thanhPho, viDo, kinhDo, soSao, trangThai, null, pageIndex, pageSize);
        }

        public async Task<(List<KhachSan> Items, int TotalCount)> LayDanhSachKhachSanOwner(
            Guid ownerId,
            int pageIndex,
            int pageSize)
        {
            IQueryable<KhachSan> query =
                _context.KhachSans.AsNoTracking();

            if (ownerId == Guid.Empty)
            {
                throw new ArgumentException("OwnerId không hợp lệ");
            }

            query = query.Where(x => x.NguoiTao == ownerId);

            int totalCount = await query.CountAsync();

            int skip = (pageIndex - 1) * pageSize;

            var items = await query
                .OrderByDescending(x => x.NgayTao)
                .Skip(Math.Max(skip, 0))
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // Private helper dùng chung cho cả Admin và Owner
        private async Task<(List<KhachSan> Items, int TotalCount)> LayDanhSachKhachSanInternal(
            string? keyword,
            string? thanhPho,
            double viDo,
            double kinhDo,
            int soSao,
            string? trangThai,
            Guid? ownerId,
            int pageIndex,
            int pageSize)
        {
            // 1. Khai báo Query với AsNoTracking để đọc nhanh hơn
            IQueryable<KhachSan> query = _context.KhachSans.AsNoTracking();

            // 2. Nếu có ownerId (role Owner) thì lọc theo Chủ sở hữu
            if (ownerId.HasValue && ownerId.Value != Guid.Empty)
            {
                query = query.Where(x => x.NguoiTao == ownerId.Value);
            }

            // 3. Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.TenKhachSan.Contains(keyword));
            }

            // 4. Lọc theo Thành phố
            if (!string.IsNullOrEmpty(thanhPho))
            {
                query = query.Where(x => x.ThanhPho == thanhPho);
            }

            // 5. Lọc theo Số sao
            if (soSao > 0)
            {
                query = query.Where(x => x.SoSao == soSao);
            }

            // 6. Lọc theo Trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(x => x.TrangThai == trangThai);
            }

            // Note: Nếu cần dùng viDo, kinhDo để lọc theo bán kính (VD: 10km) thì thêm query tại đây.

            // 7. Đếm tổng số lượng bản ghi thỏa điều kiện
            int totalCount = await query.CountAsync();

            // 8. Phân trang Skip/Take
            int skip = (pageIndex - 1) * pageSize;

            var items = await query
                .OrderByDescending(x => x.NgayTao)
                .Skip(skip < 0 ? 0 : skip)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}

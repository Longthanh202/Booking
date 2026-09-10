using Booking.Common.Shared.Enum;
using Booking.Common.Shared.Enum.Booking;
using Booking.Core.Model.KhachSans;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;

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
                .FirstOrDefaultAsync(ks => ks.Id == id) ?? null;
        }

        public async Task<(List<KhachSan> Items, int TotalCount)> FilterHotels(
            string? keyword,
            int? soKhach,
            DateTime? ngayNhanPhong,
            DateTime? ngayTraPhong,
            int pageIndex,
            int pageSize)
        {

            IQueryable<KhachSan> query = _context.KhachSans.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();

                query =
                    from ks in query
                    join p in _context.Provinces
                        on ks.ThanhPho equals p.code into pGroup
                    from p in pGroup.DefaultIfEmpty()
                    where ks.TenKhachSan.Contains(kw)
                          || (p != null && p.name.Contains(kw))
                    select ks;
            }

            if (soKhach.HasValue)
            {
                query = query.Where(x => x.LoaiPhongs.Any(lp => lp.SoKhachToiDa >= soKhach.Value));
            }

            if (ngayNhanPhong.HasValue && ngayTraPhong.HasValue)
            {
                string trangThaiDaHuy = TrangThaiDatPhong.DA_HUY.ToString();
                query = query.Where(x => x.LoaiPhongs.Any(lp =>
                    !lp.ChiTietDatPhongs.Any(ct =>
                        ct.DatPhong.NgayNhanPhong < ngayTraPhong.Value &&
                        ct.DatPhong.NgayTraPhong > ngayNhanPhong.Value &&
                        ct.DatPhong.TrangThai != trangThaiDaHuy)));
            }

            var advertisingScores =
               from ksqc in _context.KhachSanQuangCaos
               join gqc in _context.GoiQuangCaos
                   on ksqc.GoiQuanCaoId equals gqc.Id
               where ksqc.TrangThai == TrangThaiQuangCao.DANG_HIEN_THI.ToString()
               group gqc by ksqc.KhachSanId into grouped
               select new
               {
                   KhachSanId = grouped.Key,
                   DiemUuTien = grouped.Max(x => x.DiemUuTien)
               };

            var queryWithPriority =
               from ks in query
               join qc in advertisingScores
                   on ks.Id equals qc.KhachSanId into qcGroup
               from qc in qcGroup.DefaultIfEmpty()
               select new
               {
                   KhachSan = ks,
                   DiemUuTien = qc != null
                       ? qc.DiemUuTien
                       : 0
               };

            int totalCount = await query.CountAsync();
            if (totalCount == 0)
            {
                return (new List<KhachSan>(), 0);
            }

            int skip = (pageIndex - 1) * pageSize;
            var items = await query
                .Include(x => x.LoaiPhongs)
                .AsSplitQuery()
                .OrderByDescending(x => x.NgayTao)
                .Skip(skip < 0 ? 0 : skip)
                .Take(pageSize)
                .ToListAsync();

            return (items ?? new List<KhachSan>(), totalCount);
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

using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.TienIchs;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.KhachSans
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
                .Include(ks => ks.Province)
                .Include(ks => ks.LoaiPhongs)
                .Include(ks => ks.KhachSanImages)
                .Include(ks => ks.KhachSan_TienIches)
                    .ThenInclude(kst => kst.TienIch) // Đi sâu vào lấy data bảng TienIch
                .FirstOrDefaultAsync(ks => ks.Id == id);
        }

        public async Task<List<KhachSan>> FilterHotels(string? keyword, string provinceCode, int? soKhach, DateTime? ngayNhanPhong, DateTime? ngayTraPhong, int startRow, int endRow)
        {
            IQueryable<KhachSan> query = _context.KhachSans
        .Include(x => x.LoaiPhongs);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.TenKhachSan.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(provinceCode))
            {
                query = query.Where(x => x.ThanhPho == provinceCode);
            }

            if (soKhach.HasValue)
            {
                query = query.Where(x =>
                    x.LoaiPhongs.Any(lp => lp.SoKhachToiDa >= soKhach));
            }

            if (ngayNhanPhong.HasValue && ngayTraPhong.HasValue)
            {
                query = query.Where(x =>
                    x.LoaiPhongs.Any(lp =>
                        !lp.ChiTietDatPhongs.Any(ct =>
                            ct.DatPhong.NgayNhanPhong < ngayTraPhong &&
                            ct.DatPhong.NgayTraPhong > ngayNhanPhong &&
                            ct.DatPhong.TrangThai != "HUY")));
            }

            return await query
                .Skip(startRow)
                .Take(endRow - startRow)
                .ToListAsync();
        }

        public async Task<List<KhachSan>> LayDanhSachKhachSanAdmin(string keyword, string thanhPho, double viDo, double kinhDo, int soSao, string trangThai, int startRow, int endRow)
        {
            IQueryable<KhachSan> query = _context.KhachSans;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.TenKhachSan.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(thanhPho))
            {
                query = query.Where(x => x.ThanhPho == thanhPho);
            }

            if (soSao > 0)
            {
                query = query.Where(x => x.SoSao == soSao);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(x => x.TrangThai == trangThai);
            }

            return await query
                .OrderByDescending(x => x.NgayTao)
                .Skip(startRow)
                .Take(endRow - startRow)
                .ToListAsync();
        }

        public async Task<List<KhachSan>> LayDanhSachKhachSanOwner(string keyword, string thanhPho, double viDo, double kinhDo, int soSao, string trangThai, Guid ownerId, int startRow, int endRow)
        {
            IQueryable<KhachSan> query = _context.KhachSans.Where(x => x.NguoiTao == ownerId);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.TenKhachSan.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(thanhPho))
            {
                query = query.Where(x => x.ThanhPho == thanhPho);
            }

            if (soSao > 0)
            {
                query = query.Where(x => x.SoSao == soSao);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(x => x.TrangThai == trangThai);
            }

            return await query
                .OrderByDescending(x => x.NgayTao)
                .Skip(startRow)
                .Take(endRow - startRow)
                .ToListAsync();
        }

        public async Task<KhachSan> TaoKhachSan(KhachSan ks)
        {
            ks.Id = Guid.NewGuid();

            await _context.KhachSans.AddAsync(ks);

            return ks;
        }
    }
}

using Container_App.Common.Shared.Enum.Booking;
using Container_App.Core.Model.DatPhongs;
using Container_App.Core.Model.PhongDats;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.DatPhongs
{
    public class DatPhongRepository : IDatPhongRepository
    {
        private readonly AppDbContext _context;
        public DatPhongRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task CapNhatTrangThai(DatPhong d)
        {
             _context.DatPhongs.Update(d);
            return Task.CompletedTask;
        }

        public async Task<(List<DatPhong> Items, int TotalCount)> GetListBookingOwner(
            Guid ownerId,
            Guid khachSanId,
            int pageIndex,
            int pageSize)
        {
            var query = _context.DatPhongs
                .AsNoTracking()

                .Include(x => x.KhachSan)

                .Include(x => x.ThanhToans)

                .Include(x => x.ChiTietDatPhongs)
                .ThenInclude(x => x.LoaiPhong)

                .Include(x => x.ChiTietDatPhongs)
                .ThenInclude(x => x.PhongDats)
                .ThenInclude(x => x.Phong)

                .Where(x =>
                    x.KhachSanId == khachSanId &&
                    x.KhachSan.NguoiTao == ownerId
                );

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.NgayTao)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<DatPhong> Items, int TotalCount)> BookingHistory(Guid userId, int pageIndex, int pageSize)
        {
            var query = _context.DatPhongs
                .AsNoTracking()

                .Include(x => x.KhachSan)

                .Include(x => x.ThanhToans)

                .Include(x => x.ChiTietDatPhongs)
                .ThenInclude(x => x.LoaiPhong)

                .Include(x => x.ChiTietDatPhongs)
                .ThenInclude(x => x.PhongDats)
                .ThenInclude(x => x.Phong)

                .Where(x =>
                    x.KhachHangId == userId
                );

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.NgayTao)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<DatPhong> CheckIn(Guid id)
        {
            var datPhong = await _context.DatPhongs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (datPhong == null)
            {
                throw new Exception("Không tìm thấy đặt phòng");
            }

            datPhong.TrangThai = TrangThaiDatPhong.DA_CHECK_IN.ToString();

            await _context.SaveChangesAsync();

            return datPhong;
        }

        public async Task<DatPhong> XacNhan(Guid id)
        {
            var datPhong = await _context.DatPhongs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (datPhong == null)
            {
                throw new Exception("Không tìm thấy đặt phòng");
            }

            datPhong.TrangThai = TrangThaiDatPhong.DA_XAC_NHAN.ToString();

            await _context.SaveChangesAsync();

            return datPhong;
        }

        public async Task<DatPhong> DatPhong(DatPhong dp, List<ChiTietDatPhong> ctdp, List<PhongDat> phongDats)
        {
            // 1. Thêm Đặt phòng chính
            await _context.DatPhongs.AddAsync(dp);

            // 2. Thêm danh sách Chi tiết đặt phòng (dùng AddRangeAsync ngắn gọn hơn foreach)
            if (ctdp != null && ctdp.Any())
            {
                await _context.ChiTietDatPhongs.AddRangeAsync(ctdp);
            }

            // 3. Thêm danh sách Phòng đặt (gán cụ thể từng PhongId cho ChiTietDatPhongId)
            if (phongDats != null && phongDats.Any())
            {
                await _context.phongDats.AddRangeAsync(phongDats);
            }

            // Lưu ý: Không gọi SaveChangesAsync() ở đây nếu bạn đang sử dụng UnitOfWork 
            // vì UnitOfWork sẽ đảm nhận việc Commit Transaction ở lớp Service.

            return dp;
        }
        public async Task<DatPhong?> LayTheoId(Guid id)
        {
            return await _context.DatPhongs
                .Include(x => x.KhachSan)
                .Include(x => x.ThanhToans)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}

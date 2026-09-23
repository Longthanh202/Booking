using Booking.Common.Shared.Enum.Booking;
using Booking.Common.Shared.Enum.Hotel;
using Booking.Core.Model.Phongs;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Phongs
{
    internal class PhongRepository : IPhongRepository
    {
        private readonly AppDbContext _context;
        public PhongRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Phong>> LayDanhSachPhongTrong(Guid loaiPhongId, DateTime? ngayNhan, DateTime? ngayTra)
        {
            if (!ngayNhan.HasValue || !ngayTra.HasValue || ngayNhan >= ngayTra)
            {
                throw new ArgumentException("Khoảng thời gian nhận và trả phòng không hợp lệ.");
            }

            return await _context.Phongs
                .Where(p => p.LoaiPhongId == loaiPhongId && p.TrangThai == TrangThaiPhong.DANG_SU_DUNG.ToString())
                .Where(p => !_context.phongDats.Any(pd =>
                    pd.PhongId == p.Id &&
                    pd.ChiTietDatPhong != null &&
                    pd.ChiTietDatPhong.DatPhong != null &&
                    pd.ChiTietDatPhong.DatPhong.TrangThai != TrangThaiDatPhong.DA_HUY.ToString() &&
                    pd.ChiTietDatPhong.DatPhong.NgayNhanPhong < ngayTra.Value &&
                    pd.ChiTietDatPhong.DatPhong.NgayTraPhong > ngayNhan.Value
                ))
                .ToListAsync();
        }

        public async Task<Phong> CreateRoom(Phong p)
        {
            await _context.AddAsync(p);
            await _context.SaveChangesAsync();
            return p;
        }

        public async Task<Phong?> GetRoomForOwner(Guid roomId, Guid ownerId)
        {
            return await _context.Phongs
                .Include(x => x.LoaiPhong)
                    .ThenInclude(x => x!.KhachSan)
                .FirstOrDefaultAsync(x => x.Id == roomId &&
                    x.LoaiPhong != null &&
                    x.LoaiPhong.KhachSan != null &&
                    x.LoaiPhong.KhachSan.NguoiTao == ownerId);
        }

        public async Task<List<Phong>> GetRoomsByRoomType(Guid roomTypeId, Guid ownerId)
        {
            return await _context.Phongs
                .Include(x => x.LoaiPhong)
                .Where(x => x.LoaiPhongId == roomTypeId &&
                    x.LoaiPhong != null &&
                    x.LoaiPhong.KhachSan != null &&
                    x.LoaiPhong.KhachSan.NguoiTao == ownerId)
                .OrderBy(x => x.SoPhong)
                .ToListAsync();
        }

        public async Task<Phong?> UpdateRoom(Phong p)
        {
            _context.Phongs.Update(p);
            await _context.SaveChangesAsync();
            return p;
        }

        public async Task<bool> DeleteRoom(Guid roomId, Guid ownerId)
        {
            var room = await GetRoomForOwner(roomId, ownerId);
            if (room == null || await _context.phongDats.AnyAsync(x => x.PhongId == roomId))
            {
                return false;
            }

            _context.Phongs.Remove(room);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

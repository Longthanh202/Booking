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
            try
            {
                return await _context.Phongs
                .Where(p => p.LoaiPhongId == loaiPhongId && p.TrangThai == TrangThaiPhong.DANG_SU_DUNG.ToString())
                .Where(p => !_context.phongDats.Any(pd =>
                    pd.PhongId == p.Id &&
                    pd.ChiTietDatPhong.DatPhong.TrangThai != TrangThaiDatPhong.DA_HUY.ToString() &&
                    pd.ChiTietDatPhong.DatPhong.NgayNhanPhong < ngayTra &&
                    pd.ChiTietDatPhong.DatPhong.NgayTraPhong > ngayNhan
                ))
                .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new List<Phong>();
            }
        }

        public async Task<Phong> TaoPhong(Phong p)
        {
            await _context.AddAsync(p);
            await _context.SaveChangesAsync();
            return p;
        }
    }
}

using Booking.Core.Model.GiaPhongs;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.GiaPhongs
{
    public class GiaPhongRepository : IGiaPhongRepository
    {
        private readonly AppDbContext _context;
        public GiaPhongRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GiaPhong?> LayGiaPhongHienTai(Guid loaiPhongId)
        {
            var now = DateTime.Now;

            return await _context.GiaPhongs
                .Where(x =>
                    x.LoaiPhongId == loaiPhongId &&
                    x.IsActive &&
                    x.NgayBatDau <= now &&
                    (!x.NgayKetThuc.HasValue || x.NgayKetThuc.Value > now))
                .OrderByDescending(x => x.NgayBatDau)
                .FirstOrDefaultAsync();
        }

        public async Task<List<GiaPhong>> LayGiaPhongTheoDSKhachSanId(List<Guid> ids)
        {
            return await _context.GiaPhongs
                .Where(x =>
                    x.IsActive == true &&
                    x.LoaiPhong.KhachSanId.HasValue &&
                    ids.Contains(x.LoaiPhong.KhachSanId.Value))
                .GroupBy(x => x.LoaiPhong.KhachSanId!.Value)
                .Select(g => g.OrderBy(x => x.Gia).First())
                .ToListAsync();
        }
    }
}

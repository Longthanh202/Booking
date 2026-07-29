using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container_App.Core.Model.ChiTietHoaDonHoaHongs;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;


namespace Container_App.Data.Repository.ChiTietHoaDonHoaHongs
{
   
    public class ChiTietHoaDonHoaHongRepository : IChiTietHoaDonHoaHongRepository
    {
        private readonly AppDbContext _context;

        public ChiTietHoaDonHoaHongRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChiTietHoaDonHoaHong?> LayTheoId(long id)
        {
            return await _context.ChiTietHoaDonHoaHongs
                .Include(x => x.HoaDonHoaHong)
                .Include(x => x.HoaHong)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ChiTietHoaDonHoaHong>> LayTheoHoaDon(long hoaDonHoaHongId)
        {
            return await _context.ChiTietHoaDonHoaHongs
                .Where(x => x.HoaDonHoaHongId == hoaDonHoaHongId)
                .Include(x => x.HoaHong)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChiTietHoaDonHoaHong>> LayTheoHoaHong(long hoaHongId)
        {
            return await _context.ChiTietHoaDonHoaHongs
                .Where(x => x.HoaHongId == hoaHongId)
                .Include(x => x.HoaDonHoaHong)
                .ToListAsync();
        }

        public async Task Tao(ChiTietHoaDonHoaHong chiTiet)
        {
            await _context.ChiTietHoaDonHoaHongs.AddAsync(chiTiet);
        }

        public async Task TaoDanhSach(IEnumerable<ChiTietHoaDonHoaHong> danhSach)
        {
            await _context.ChiTietHoaDonHoaHongs.AddRangeAsync(danhSach);
        }

        public async Task Xoa(long id)
        {
            var entity = await _context.ChiTietHoaDonHoaHongs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity != null)
            {
                _context.ChiTietHoaDonHoaHongs.Remove(entity);
            }
        }
    }
}

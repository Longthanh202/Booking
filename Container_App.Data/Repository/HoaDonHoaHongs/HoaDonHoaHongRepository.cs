using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container_App.Core.Model.HoaDonHoaHongs;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;


namespace Container_App.Data.Repository.HoaDonHoaHongs
{
   
    public class HoaDonHoaHongRepository : IHoaDonHoaHongRepository
    {
        private readonly AppDbContext _context;

        public HoaDonHoaHongRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HoaDonHoaHong?> LayTheoId(long id)
        {
            return await _context.HoaDonHoaHongs
                .Include(x => x.KhachSan)
                .Include(x => x.ChiTietHoaDonHoaHongs)
                    .ThenInclude(x => x.HoaHong)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<HoaDonHoaHong>> LayTheoKhachSan(Guid khachSanId)
        {
            return await _context.HoaDonHoaHongs
                .Where(x => x.KhachSanId == khachSanId)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }

        public async Task<IEnumerable<HoaDonHoaHong>> LayTheoTrangThai(string trangThai)
        {
            return await _context.HoaDonHoaHongs
                .Where(x => x.TrangThai == trangThai)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }

        public async Task<IEnumerable<HoaDonHoaHong>> LayTheoKhoangNgay(
            DateTime tuNgay,
            DateTime denNgay)
        {
            return await _context.HoaDonHoaHongs
                .Where(x => x.TuNgay >= tuNgay &&
                            x.DenNgay <= denNgay)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }

        public async Task Tao(HoaDonHoaHong hoaDon)
        {
            await _context.HoaDonHoaHongs.AddAsync(hoaDon);
        }

        public Task CapNhat(HoaDonHoaHong hoaDon)
        {
            _context.HoaDonHoaHongs.Update(hoaDon);
            return Task.CompletedTask;
        }

        public async Task Xoa(long id)
        {
            var entity = await _context.HoaDonHoaHongs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity != null)
            {
                _context.HoaDonHoaHongs.Remove(entity);
            }
        }
    }
}

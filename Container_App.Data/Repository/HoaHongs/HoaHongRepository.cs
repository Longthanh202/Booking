using Container_App.Core.Model.HoaHongs;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.HoaHongs
{
    internal class HoaHongRepository : IHoaHongRepository
    {
        private readonly AppDbContext _context;

        public HoaHongRepository(AppDbContext context)
        {
            _context = context;
        }   

        public Task<IEnumerable<HoaHong>> LayChuaThu()
        {
            throw new NotImplementedException();
        }

        public async Task<HoaHong?> LayTheoDatPhong(Guid datPhongId)
        {
            return await _context.HoaHongs
                .FirstOrDefaultAsync(x => x.MaDatPhong == datPhongId);
        }


        public async Task<HoaHong?> LayTheoId(long id)
        {
            return await _context.HoaHongs
                .Include(x => x.KhachSan)
                .Include(x => x.DatPhong)
                .FirstOrDefaultAsync(x => x.MaHoaHong == id);
        }

        public async Task<IEnumerable<HoaHong>> LayTheoKhachSan(Guid khachSanId)
        {
            return await _context.HoaHongs
                .Where(x => x.MaKhachSan == khachSanId)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }

        public async Task Tao(HoaHong hoaHong)
        {
            await _context.HoaHongs.AddAsync(hoaHong);
        }

        public Task CapNhat(HoaHong hoaHong)
        {
            _context.HoaHongs.Update(hoaHong);

            return Task.CompletedTask;
        }

        public async Task Xoa(long id)
        {
            var entity = await _context.HoaHongs
                .FirstOrDefaultAsync(x => x.MaHoaHong == id);

            if (entity != null)
            {
                _context.HoaHongs.Remove(entity);
            }
        }
    }
}

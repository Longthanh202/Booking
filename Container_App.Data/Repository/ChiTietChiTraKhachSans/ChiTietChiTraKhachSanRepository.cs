using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container_App.Core.Model.ChiTietChiTraKhachSans;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;


namespace Container_App.Data.Repository.ChiTietChiTraKhachSans
{
   
    public class ChiTietChiTraKhachSanRepository : IChiTietChiTraKhachSanRepository
    {
        private readonly AppDbContext _context;

        public ChiTietChiTraKhachSanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChiTietChiTraKhachSan?> LayTheoId(long id)
        {
            return await _context.ChiTietChiTraKhachSans
                .Include(x => x.ChiTraKhachSan)
                .Include(x => x.DatPhong)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ChiTietChiTraKhachSan>> LayTheoChiTra(long chiTraKhachSanId)
        {
            return await _context.ChiTietChiTraKhachSans
                .Where(x => x.ChiTraKhachSanId == chiTraKhachSanId)
                .Include(x => x.DatPhong)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChiTietChiTraKhachSan>> LayTheoDatPhong(Guid datPhongId)
        {
            return await _context.ChiTietChiTraKhachSans
                .Where(x => x.DatPhongId == datPhongId)
                .Include(x => x.ChiTraKhachSan)
                .ToListAsync();
        }

        public async Task Tao(ChiTietChiTraKhachSan chiTietChiTraKhachSan)
        {
            await _context.ChiTietChiTraKhachSans.AddAsync(chiTietChiTraKhachSan);
        }

        public async Task TaoDanhSach(IEnumerable<ChiTietChiTraKhachSan> danhSach)
        {
            await _context.ChiTietChiTraKhachSans.AddRangeAsync(danhSach);
        }

        public async Task Xoa(long id)
        {
            var entity = await _context.ChiTietChiTraKhachSans
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity != null)
            {
                _context.ChiTietChiTraKhachSans.Remove(entity);
            }
        }
    }
}

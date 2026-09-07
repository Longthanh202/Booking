using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Common.Shared;
using Booking.Core.Model.ChiTietChiTraKhachSans;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;


namespace Booking.Data.Repository.ChiTietChiTraKhachSans
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
            try
            {
                return await _context.ChiTietChiTraKhachSans
                .Include(x => x.ChiTraKhachSan)
                .Include(x => x.DatPhong)
                .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch(Exception ex)
            {
                FileLogger.Log(ex);
                return null;
            }
        }

        public async Task<IEnumerable<ChiTietChiTraKhachSan>> LayTheoChiTra(long chiTraKhachSanId)
        {
            try
            {
                return await _context.ChiTietChiTraKhachSans
                .Where(x => x.ChiTraKhachSanId == chiTraKhachSanId)
                .Include(x => x.DatPhong)
                .OrderBy(x => x.Id)
                .ToListAsync();
            }catch(Exception ex)
            {
                FileLogger.Log(ex);
                return null;
            }
        }

        public async Task<IEnumerable<ChiTietChiTraKhachSan>> LayTheoDatPhong(Guid datPhongId)
        {
            try
            {
                return await _context.ChiTietChiTraKhachSans
                .Where(x => x.DatPhongId == datPhongId)
                .Include(x => x.ChiTraKhachSan)
                .ToListAsync();
            }
            catch(Exception ex)
            {
                FileLogger.Log(ex);
                return null;
            }
        }

        public async Task Tao(ChiTietChiTraKhachSan chiTietChiTraKhachSan)
        {
            try
            {
                await _context.ChiTietChiTraKhachSans.AddAsync(chiTietChiTraKhachSan);
            }catch(Exception ex)
            {
                FileLogger.Log(ex);
            }
        }

        public async Task TaoDanhSach(IEnumerable<ChiTietChiTraKhachSan> danhSach)
        {
            try
            {
                await _context.ChiTietChiTraKhachSans.AddRangeAsync(danhSach);
            }catch(Exception ex)
            {
                FileLogger.Log(ex);
            }
        }

        public async Task Xoa(long id)
        {
            try
            {
                var entity = await _context.ChiTietChiTraKhachSans
                .FirstOrDefaultAsync(x => x.Id == id);

                if (entity != null)
                {
                    _context.ChiTietChiTraKhachSans.Remove(entity);
                }
            }catch(Exception ex)
            {
                FileLogger.Log(ex);
            }
        }
    }
}

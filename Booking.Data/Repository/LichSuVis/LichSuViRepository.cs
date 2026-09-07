using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Core.Model.LichSuVis;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data.Repository.LichSuVis
{
    public class LichSuViRepository : ILichSuViRepository
    {
        private readonly AppDbContext _context;

        public LichSuViRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LichSuVi?> LayTheoId(long id)
        {
            return await _context.LichSuVis
                .Include(x => x.ViKhachSan)
                .Include(x => x.DatPhong)
                .FirstOrDefaultAsync(x => x.MaLichSu == id);
        }

        public async Task<IEnumerable<LichSuVi>> LayTheoVi(long viKhachSanId)
        {
            return await _context.LichSuVis
                .Where(x => x.MaVi == viKhachSanId)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }

        public async Task<IEnumerable<LichSuVi>> LayTheoDatPhong(Guid datPhongId)
        {
            return await _context.LichSuVis
                .Where(x => x.MaDatPhong == datPhongId)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }

        public async Task<IEnumerable<LichSuVi>> LayTheoOwnerId(Guid ownerId)
        {
            return await _context.LichSuVis
                .Include(x => x.DatPhong)
                .ThenInclude(x => x.KhachSan)
                .Where(x => x.DatPhong.KhachSan.NguoiTao == ownerId)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }

        public async Task<IEnumerable<LichSuVi>> LayTheoKhoangThoiGian(
            long viKhachSanId,
            DateTime tuNgay,
            DateTime denNgay)
        {
            return await _context.LichSuVis
                .Where(x =>
                    x.MaVi == viKhachSanId &&
                    x.NgayTao >= tuNgay &&
                    x.NgayTao <= denNgay)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }

        public async Task Tao(LichSuVi lichSuVi)
        {
            await _context.LichSuVis.AddAsync(lichSuVi);
        }
    }
}

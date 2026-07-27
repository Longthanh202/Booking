using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container_App.Common.Shared.Enum;
using Container_App.Core.Model.ChiTraKhachSans;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Container_App.Data.Repository.ChiTraKhachSans
{
    public class ChiTraKhachSanRepository : IChiTraKhachSanRepository
    {
        private readonly AppDbContext _context;

        public ChiTraKhachSanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChiTraKhachSan?> LayTheoId(long id)
        {
            return await _context.ChiTraKhachSans
                .Include(x => x.KhachSan)
                .Include(x => x.TaiKhoanNganHang)
                .Include(x => x.ChiTietChiTraKhachSans)
                .FirstOrDefaultAsync(x => x.MaChiTra == id);
        }

        public async Task<IEnumerable<ChiTraKhachSan>> LayTheoKhachSan(Guid khachSanId)
        {
            return await _context.ChiTraKhachSans
                .Where(x => x.KhachSanId == khachSanId)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChiTraKhachSan>> LayTheoTrangThai(string trangThai)
        {
            return await _context.ChiTraKhachSans
                .Where(x => x.TrangThai == trangThai)
                .OrderBy(x => x.NgayTao)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChiTraKhachSan>> LayChoChiTra()
        {
            return await _context.ChiTraKhachSans
                .Where(x => x.TrangThai == TrangThaiChiTraKhachSan.CHO_CHUYEN.ToString())
                .OrderBy(x => x.NgayTao)
                .ToListAsync();
        }

        public async Task Tao(ChiTraKhachSan chiTraKhachSan)
        {
            await _context.ChiTraKhachSans.AddAsync(chiTraKhachSan);
        }

        public Task CapNhat(ChiTraKhachSan chiTraKhachSan)
        {
            _context.ChiTraKhachSans.Update(chiTraKhachSan);

            return Task.CompletedTask;
        }

        public async Task Xoa(long id)
        {
            var entity = await _context.ChiTraKhachSans
                .FirstOrDefaultAsync(x => x.MaChiTra == id);

            if (entity != null)
            {
                _context.ChiTraKhachSans.Remove(entity);
            }
        }
    }
}

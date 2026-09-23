using Booking.Core.Model.ViKhachSans;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.ViKhachSans
{
    internal class ViKhachSanRepository: IViKhachSanRepository
    {
        private readonly AppDbContext _context;

        public ViKhachSanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ViKhachSan?> LayTheoId(long id)
        {
            return await _context.ViKhachSans
                .Include(x => x.KhachSan)
                .FirstOrDefaultAsync(x => x.MaVi == id);
        }

        public async Task<ViKhachSan?> LayTheoKhachSan(Guid khachSanId)
        {
            return await _context.ViKhachSans
                .Include(x => x.KhachSan)
                .FirstOrDefaultAsync(x => x.MaKhachSan == khachSanId);
        }

        public Task<List<ViKhachSan>> LayTheoOwner(Guid ownerId)
        {
            return _context.ViKhachSans
                .Include(x => x.KhachSan)
                .Where(x => x.KhachSan != null && x.KhachSan.NguoiTao == ownerId)
                .OrderBy(x => x.MaKhachSan)
                .ToListAsync();
        }

        public Task<ViKhachSan?> LayTheoKhachSanCuaOwner(Guid khachSanId, Guid ownerId)
        {
            return _context.ViKhachSans
                .Include(x => x.KhachSan)
                .FirstOrDefaultAsync(x => x.MaKhachSan == khachSanId &&
                    x.KhachSan != null && x.KhachSan.NguoiTao == ownerId);
        }

        public async Task Tao(ViKhachSan viKhachSan)
        {
            await _context.ViKhachSans.AddAsync(viKhachSan);
        }

        public Task CapNhat(ViKhachSan viKhachSan)
        {
            _context.ViKhachSans.Update(viKhachSan);

            return Task.CompletedTask;
        }
    }
}

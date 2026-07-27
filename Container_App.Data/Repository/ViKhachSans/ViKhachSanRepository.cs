using Container_App.Core.Model.ViKhachSans;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.ViKhachSans
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

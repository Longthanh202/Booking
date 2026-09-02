using Booking.Core.Model.TienIchs;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.TienIchs
{
    internal class TienIchRepository : ITienIchRepository
    {
        private readonly AppDbContext _context;
        public TienIchRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<TienIch>> GetTienIchKhachSanByKhachSanId(Guid khachSanId)
        {
            return await _context.KhachSan_TienIches
                .Where(kst => kst.KhachSanId == khachSanId)
                .Select(kst => new TienIch
                {
                    Id = kst.TienIch.Id,
                    TenTienIch = kst.TienIch.TenTienIch,
                    Icon = kst.TienIch.Icon
                })
                .ToListAsync();
        }

        public async Task<TienIch> ThemTienIch(TienIch tienIch)
        {
            await _context.TienIches.AddAsync(tienIch);       
            return tienIch;
        }
    }
}

using Booking.Core.Model.TienIchs;
using Booking.Data.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.KhachSanTienIch
{
    public class KhachSanTienIchRepository : IKhachSanTienIchRepository
    {
        private readonly AppDbContext _context;
        public KhachSanTienIchRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<KhachSan_TienIch> Tao(KhachSan_TienIch entity)
        {
            await _context.AddAsync(entity);
            return entity;
        }
    }
}

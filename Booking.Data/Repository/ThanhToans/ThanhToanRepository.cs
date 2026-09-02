using Booking.Core.Model.DatPhongs;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.ThanhToans
{
    public class ThanhToanRepository : IThanhToanRepository
    {
        private readonly AppDbContext _context;
        public ThanhToanRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ThanhToan> Tao(ThanhToan t)
        {
            await _context.AddAsync(t);
            return t;
        }

        public async Task<ThanhToan?> LayTheoDatPhong(Guid datPhongId)
        {
            return await _context.ThanhToans
                .FirstOrDefaultAsync(x => x.DatPhongId == datPhongId);
        }
    }
}

using Booking.Core.Model.DatPhongs;
using Booking.Core.Model.PhongDats;
using Booking.Data.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.PhongDats
{
    public class PhongDatRepository : IPhongDatRepository
    {
        private readonly AppDbContext _context;
        public PhongDatRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PhongDat> Tao(PhongDat phongDat)
        {
            await _context.AddAsync(phongDat);
            return phongDat;
        }
    }
}

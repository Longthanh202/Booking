using Container_App.Core.Model.Phongs;
using Container_App.Data.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Phongs
{
    internal class PhongRepository : IPhongRepository
    {
        private readonly AppDbContext _context;
        public PhongRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Phong> TaoPhong(Phong p)
        {
            await _context.AddAsync(p);
            await _context.SaveChangesAsync();
            return p;
        }
    }
}

using Booking.Core.Model.Provinces;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Provinces
{
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly AppDbContext _dbContext;
        public ProvinceRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Province>> GetProvinces()
        {
            return await _dbContext.Provinces.ToListAsync();
        }
    }
}

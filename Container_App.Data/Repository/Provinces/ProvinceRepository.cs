using Container_App.Core.Model.Provinces;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Provinces
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

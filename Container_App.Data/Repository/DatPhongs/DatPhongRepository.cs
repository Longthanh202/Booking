using Container_App.Core.Model.DatPhongs;
using Container_App.Data.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.DatPhongs
{
    public class DatPhongRepository : IDatPhongRepository
    {
        private readonly AppDbContext _context;
        public DatPhongRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<DatPhong> DatPhong(DatPhong dp, List<ChiTietDatPhong> ctdp)
        {
            dp.Id = Guid.NewGuid();
            dp.TrangThai = dp.TrangThai;
            await _context.DatPhongs.AddAsync(dp);

            foreach (var item in ctdp)
            {
                item.Id = Guid.NewGuid();
                item.DatPhongId = dp.Id;
                await _context.ChiTietDatPhongs.AddAsync(item);
            }
            await _context.ChiTietDatPhongs.AddRangeAsync(ctdp);

            return dp;
        }
    }
}

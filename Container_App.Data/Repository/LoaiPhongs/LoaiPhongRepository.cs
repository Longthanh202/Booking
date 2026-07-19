using Container_App.Core.Model.LoaiPhongs;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.LoaiPhongs
{
    public class LoaiPhongRepository : ILoaiPhongRepository
    {
        private readonly AppDbContext _context;
        public LoaiPhongRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<LoaiPhong>> GetLoaiPhongByKhachSanId(Guid khachSanId)
        {
            return await _context.LoaiPhongs
                .Where(x => x.KhachSanId == khachSanId)
                .Select(x => new LoaiPhong
                {
                    Id = x.Id,
                    TenLoaiPhong = x.TenLoaiPhong,
                    SoKhachToiDa = x.SoKhachToiDa,
                    KieuGiuong = x.KieuGiuong,
                    MoTa = x.MoTa,
                })
                .ToListAsync();
        }

        public async Task<LoaiPhong> TaoLoaiPhong(LoaiPhong lp)
        {
            await _context.LoaiPhongs.AddAsync(lp);
            await _context.SaveChangesAsync();
            return lp;
        }
    }
}

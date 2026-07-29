using Container_App.Core.Model.KhachSanImage;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.KhachSanImage
{
    public class KhachSanImageRepository : IKhachSanImageRepository
    {
        private readonly AppDbContext _context;
        public KhachSanImageRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<KhachSanImages>> GetHotelImages(List<Guid> ids)
        {
            return await _context.KhachSanImages
                .Where(x => ids.Contains(x.KhachSanId))
                .ToListAsync();
        }

        public async Task<List<KhachSanImages>> GetListImageByKhachSanId(Guid khachSanId)
        {
            return await _context.KhachSanImages
                .Where(x => x.KhachSanId == khachSanId)
                .Select(x => new KhachSanImages
                {
                    Id = x.Id,
                    Url = x.Url
                })
                .ToListAsync();
        }

        public async Task InsertKhachSanImage(Guid khachSanId, List<string> urls)
        {
            if (urls == null || !urls.Any())
                return;

            var images = urls.Select(url => new KhachSanImages
            {             
                KhachSanId = khachSanId,
                Url = url,              
            }).ToList();

            await _context.KhachSanImages.AddRangeAsync(images);
        }
    }
}

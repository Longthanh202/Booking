using Booking.Core.Model.KhachSanImage;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.KhachSanImage
{
    public class KhachSanImageRepository : IKhachSanImageRepository
    {
        private readonly AppDbContext _context;
        public KhachSanImageRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<KhachSanAnh>> GetHotelImages(List<Guid> ids)
        {
            return await _context.KhachSanAnhs
                .Where(x => ids.Contains(x.KhachSanId))
                .ToListAsync();
        }

        public async Task<List<KhachSanAnh>> GetListImageByKhachSanId(Guid khachSanId)
        {
            return await _context.KhachSanAnhs
                .Where(x => x.KhachSanId == khachSanId)
                .Select(x => new KhachSanAnh
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

            var images = urls.Select(url => new KhachSanAnh
            {             
                KhachSanId = khachSanId,
                Url = url,              
            }).ToList();

            await _context.KhachSanAnhs.AddRangeAsync(images);
        }

        public Task<KhachSanAnh?> GetImageForOwner(long imageId, Guid hotelId, Guid ownerId)
        {
            return _context.KhachSanAnhs.FirstOrDefaultAsync(image =>
                image.Id == imageId && image.KhachSanId == hotelId &&
                image.KhachSan != null && image.KhachSan.NguoiTao == ownerId);
        }

        public async Task<bool> DeleteImageForOwner(long imageId, Guid hotelId, Guid ownerId)
        {
            var image = await GetImageForOwner(imageId, hotelId, ownerId);
            if (image == null) return false;
            _context.KhachSanAnhs.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

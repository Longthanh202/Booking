using Booking.Core.Model.Banners;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Banners
{
    public class BannerRepository : IBannerRepository
    {
        private readonly AppDbContext _context;

        public BannerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Banner>> GetBanners(string keyword, int isActive, int startRow, int endRow)
        {
            IQueryable<Banner> query = _context.Banners.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(b => b.Title.Contains(keyword) || b.Subtitle.Contains(keyword));
            }
            if (isActive == 1)
            {
                query = query.Where(b => b.IsActive == 1);
            }
            return await query
                .OrderByDescending(x => x.CreatedDate)
                .Skip(startRow)
                .Take(endRow - startRow)
                .ToListAsync();
        }


        public async Task<List<Banner>> GetActiveBanners()
        {
            return await _context.Banners.Where(x => x.IsActive == 1)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();
        }

        public async Task<Banner> CreateBanner(Banner banner)
        {
            await _context.Banners.AddAsync(banner);
            await _context.SaveChangesAsync();

            return banner;
        }

        public async Task<Banner?> UpdateBanner(Banner banner)
        {
            var current = await _context.Banners.FirstOrDefaultAsync(x => x.Id == banner.Id);
            if (current == null)
            {
                return null;
            }

            current.Title = banner.Title;
            current.Subtitle = banner.Subtitle;
            current.Url = banner.Url ?? current.Url;
            current.IsActive = banner.IsActive;
            await _context.SaveChangesAsync();
            return current;
        }

        public async Task<bool> DeleteBanner(long id)
        {
            var banner = await _context.Banners.FirstOrDefaultAsync(x => x.Id == id);
            if (banner == null)
            {
                return false;
            }

            _context.Banners.Remove(banner);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

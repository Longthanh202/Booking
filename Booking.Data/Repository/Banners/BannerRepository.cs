using Booking.Core.Model.Banners;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data.Repository.Banners
{
    public class BannerRepository : IBannerRepository
    {
        private readonly AppDbContext _context;

        public BannerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Banner>> GetAllBanner(string keyword, int isActive, int startRow, int endRow)
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


        public async Task<List<Banner>> GetBannerIsActive()
        {
            return await _context.Banners.Where(x => x.IsActive == 1)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();
        }

        public async Task<Banner> InsertBanner(Banner banner)
        {
            await _context.Banners.AddAsync(banner);
            await _context.SaveChangesAsync();

            return banner;
        }
    }
}

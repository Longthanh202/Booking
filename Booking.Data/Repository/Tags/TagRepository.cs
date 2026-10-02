using Booking.Core.Model.KhachSans;
using Booking.Core.Model.Tags;
using Booking.Common.Shared.Enum.Hotel;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data.Repository.Tags
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Tag>> GetActiveTagsAsync()
        {
            return _context.Tags.AsNoTracking()
                .Where(tag => tag.IsActive)
                .OrderBy(tag => tag.SortOrder)
                .ThenBy(tag => tag.Name)
                .Include(tag => tag.HotelTags.Where(link =>
                    link.Hotel.TrangThai == TrangThaiKhachSan.DA_DUYET.ToString()))
                .ToListAsync();
        }

        public Task<List<Tag>> GetAllAsync()
        {
            return _context.Tags.AsNoTracking()
                .OrderBy(tag => tag.SortOrder)
                .ThenBy(tag => tag.Name)
                .Include(tag => tag.HotelTags)
                .ToListAsync();
        }

        public Task<Tag?> GetByIdAsync(Guid id) => _context.Tags.FindAsync(id).AsTask();

        public Task<bool> SlugExistsAsync(string slug, Guid? exceptId = null)
        {
            return _context.Tags.AnyAsync(tag => tag.Slug == slug && (!exceptId.HasValue || tag.Id != exceptId.Value));
        }

        public async Task<(Tag? Tag, List<KhachSan> Hotels, int TotalCount)> GetHotelsBySlugAsync(string slug, int page, int pageSize)
        {
            var tag = await _context.Tags.AsNoTracking()
                .FirstOrDefaultAsync(item => item.Slug == slug && item.IsActive);
            if (tag == null)
            {
                return (null, new List<KhachSan>(), 0);
            }

            var query = _context.KhachSans.AsNoTracking()
                .Where(hotel =>
                    hotel.TrangThai == TrangThaiKhachSan.DA_DUYET.ToString() &&
                    hotel.HotelTags.Any(link => link.TagId == tag.Id));
            var totalCount = await query.CountAsync();
            var hotels = await query
                .OrderByDescending(hotel => hotel.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(hotel => hotel.KhachSanImages)
                .Include(hotel => hotel.Province)
                .ToListAsync();

            return (tag, hotels, totalCount);
        }

        public async Task AddAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
        }

        public async Task<bool> SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReplaceHotelsAsync(Guid tagId, List<Guid> hotelIds)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(item => item.Id == tagId);
            if (tag == null)
            {
                return false;
            }

            var existingHotelCount = await _context.KhachSans.CountAsync(hotel => hotelIds.Contains(hotel.Id));
            if (existingHotelCount != hotelIds.Count)
            {
                return false;
            }

            var currentLinks = await _context.HotelTags.Where(link => link.TagId == tagId).ToListAsync();
            _context.HotelTags.RemoveRange(currentLinks);
            await _context.HotelTags.AddRangeAsync(hotelIds.Select(hotelId => new HotelTag
            {
                HotelId = hotelId,
                TagId = tagId
            }));

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
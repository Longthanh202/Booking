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

        public Task<List<TheTag>> GetActiveTagsAsync()
        {
            return _context.TheTags.AsNoTracking()
                .Where(tag => tag.IsActive)
                .OrderBy(tag => tag.SortOrder)
                .ThenBy(tag => tag.Name)
                .Include(tag => tag.KhachSan_TheTags.Where(link =>
                    link.Hotel.TrangThai == TrangThaiKhachSan.DA_DUYET.ToString()))
                .ToListAsync();
        }

        public Task<List<TheTag>> GetAllAsync()
        {
            return _context.TheTags.AsNoTracking()
                .OrderBy(tag => tag.SortOrder)
                .ThenBy(tag => tag.Name)
                .Include(tag => tag.KhachSan_TheTags)
                .ToListAsync();
        }

        public Task<TheTag?> GetByIdAsync(Guid id) => _context.TheTags.FindAsync(id).AsTask();

        public async Task<bool> AreActiveTagsAsync(IReadOnlyCollection<Guid> tagIds)
        {
            var activeTagCount = await _context.TheTags.CountAsync(tag =>
                tagIds.Contains(tag.Id) && tag.IsActive);
            return activeTagCount == tagIds.Count;
        }

        public Task<bool> SlugExistsAsync(string slug, Guid? exceptId = null)
        {
            return _context.TheTags.AnyAsync(tag => tag.Slug == slug && (!exceptId.HasValue || tag.Id != exceptId.Value));
        }

        public async Task<(TheTag? TheTag, List<KhachSan> Hotels, int TotalCount)> GetHotelsBySlugAsync(string slug, int page, int pageSize)
        {
            var tag = await _context.TheTags.AsNoTracking()
                .FirstOrDefaultAsync(item => item.Slug == slug && item.IsActive);
            if (tag == null)
            {
                return (null, new List<KhachSan>(), 0);
            }

            var query = _context.KhachSans.AsNoTracking()
                .Where(hotel =>
                    hotel.TrangThai == TrangThaiKhachSan.DA_DUYET.ToString() &&
                    hotel.KhachSan_TheTags.Any(link => link.TagId == tag.Id));
            var totalCount = await query.CountAsync();
            var hotels = await query
                .OrderByDescending(hotel => hotel.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(hotel => hotel.KhachSanAnhs)
                .Include(hotel => hotel.TinhThanh)
                .ToListAsync();

            return (tag, hotels, totalCount);
        }

        public async Task AddAsync(TheTag tag)
        {
            await _context.TheTags.AddAsync(tag);
        }

        public async Task<bool> SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReplaceHotelsAsync(Guid tagId, List<Guid> hotelIds)
        {
            var tag = await _context.TheTags.FirstOrDefaultAsync(item => item.Id == tagId);
            if (tag == null)
            {
                return false;
            }

            var existingHotelCount = await _context.KhachSans.CountAsync(hotel => hotelIds.Contains(hotel.Id));
            if (existingHotelCount != hotelIds.Count)
            {
                return false;
            }

            var currentLinks = await _context.KhachSan_TheTags.Where(link => link.TagId == tagId).ToListAsync();
            _context.KhachSan_TheTags.RemoveRange(currentLinks);
            await _context.KhachSan_TheTags.AddRangeAsync(hotelIds.Select(hotelId => new KhachSan_TheTag
            {
                HotelId = hotelId,
                TagId = tagId
            }));

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

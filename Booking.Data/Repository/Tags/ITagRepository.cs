using Booking.Core.Model.KhachSans;
using Booking.Core.Model.Tags;

namespace Booking.Data.Repository.Tags
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetActiveTagsAsync();
        Task<List<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(Guid id);
        Task<bool> SlugExistsAsync(string slug, Guid? exceptId = null);
        Task<(Tag? Tag, List<KhachSan> Hotels, int TotalCount)> GetHotelsBySlugAsync(string slug, int page, int pageSize);
        Task AddAsync(Tag tag);
        Task<bool> SaveChangesAsync();
        Task<bool> ReplaceHotelsAsync(Guid tagId, List<Guid> hotelIds);
    }
}
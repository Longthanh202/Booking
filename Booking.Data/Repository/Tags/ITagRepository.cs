using Booking.Core.Model.KhachSans;
using Booking.Core.Model.Tags;

namespace Booking.Data.Repository.Tags
{
    public interface ITagRepository
    {
        Task<List<TheTag>> GetActiveTagsAsync();
        Task<List<TheTag>> GetAllAsync();
        Task<TheTag?> GetByIdAsync(Guid id);
        Task<bool> AreActiveTagsAsync(IReadOnlyCollection<Guid> tagIds);
        Task<bool> SlugExistsAsync(string slug, Guid? exceptId = null);
        Task<(TheTag? TheTag, List<KhachSan> Hotels, int TotalCount)> GetHotelsBySlugAsync(string slug, int page, int pageSize);
        Task AddAsync(TheTag tag);
        Task<bool> SaveChangesAsync();
        Task<bool> ReplaceHotelsAsync(Guid tagId, List<Guid> hotelIds);
    }
}
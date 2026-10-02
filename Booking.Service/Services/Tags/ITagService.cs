using Booking.Service.Dtos.Tags;

namespace Booking.Service.Services.Tags
{
    public interface ITagService
    {
        Task<List<TagResponse>> GetActiveTagsAsync();
        Task<List<TagResponse>> GetAllAsync();
        Task<TagHotelsPageResponse?> GetHotelsBySlugAsync(string slug, int page, int pageSize);
        Task<TagResponse?> CreateAsync(SaveTagRequest request);
        Task<TagResponse?> UpdateAsync(Guid id, SaveTagRequest request);
        Task<bool> DeactivateAsync(Guid id);
        Task<bool> ReplaceHotelsAsync(Guid tagId, List<Guid> hotelIds);
    }
}
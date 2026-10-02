using System.Text.RegularExpressions;
using Booking.Core.Model.Tags;
using Booking.Data.Repository.Tags;
using Booking.Service.Dtos.Tags;

namespace Booking.Service.Services.Tags
{
    public class TagService : ITagService
    {
        private static readonly Regex ValidSlug = new("^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled);
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<TagResponse>> GetActiveTagsAsync() => MapTags(await _tagRepository.GetActiveTagsAsync());

        public async Task<List<TagResponse>> GetAllAsync() => MapTags(await _tagRepository.GetAllAsync());

        public async Task<TagHotelsPageResponse?> GetHotelsBySlugAsync(string slug, int page, int pageSize)
        {
            var (tag, hotels, totalCount) = await _tagRepository.GetHotelsBySlugAsync(slug, page, pageSize);
            if (tag == null)
            {
                return null;
            }

            return new TagHotelsPageResponse
            {
                Tag = MapTag(tag),
                Data = hotels.Select(hotel => new TagHotelResponse
                {
                    Id = hotel.Id,
                    TenKhachSan = hotel.TenKhachSan,
                    MoTa = hotel.MoTa,
                    DiaChi = hotel.DiaChi,
                    ThanhPho = hotel.Province?.name ?? hotel.ThanhPho,
                    SoSao = hotel.SoSao,
                    TrangThai = hotel.TrangThai,
                    Urls = hotel.KhachSanImages.Where(image => image.Url != null).Select(image => image.Url!).ToList()
                }).ToList(),
                TotalRow = totalCount,
                TotalPage = (int)Math.Ceiling(totalCount / (double)pageSize),
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<TagResponse?> CreateAsync(SaveTagRequest request)
        {
            if (!IsValid(request) || await _tagRepository.SlugExistsAsync(request.Slug.Trim()))
            {
                return null;
            }

            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Slug = request.Slug.Trim(),
                Description = request.Description?.Trim(),
                SortOrder = request.SortOrder,
                IsActive = request.IsActive
            };

            await _tagRepository.AddAsync(tag);
            if (!await _tagRepository.SaveChangesAsync())
            {
                return null;
            }

            return MapTag(tag);
        }

        public async Task<TagResponse?> UpdateAsync(Guid id, SaveTagRequest request)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null || !IsValid(request) || await _tagRepository.SlugExistsAsync(request.Slug.Trim(), id))
            {
                return null;
            }

            tag.Name = request.Name.Trim();
            tag.Slug = request.Slug.Trim();
            tag.Description = request.Description?.Trim();
            tag.SortOrder = request.SortOrder;
            tag.IsActive = request.IsActive;
            return await _tagRepository.SaveChangesAsync() ? MapTag(tag) : null;
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
            {
                return false;
            }

            tag.IsActive = false;
            return await _tagRepository.SaveChangesAsync();
        }

        public Task<bool> ReplaceHotelsAsync(Guid tagId, List<Guid> hotelIds) =>
            _tagRepository.ReplaceHotelsAsync(tagId, hotelIds.Distinct().ToList());

        private static bool IsValid(SaveTagRequest request) =>
            !string.IsNullOrWhiteSpace(request.Name) &&
            !string.IsNullOrWhiteSpace(request.Slug) &&
            ValidSlug.IsMatch(request.Slug.Trim());

        private static List<TagResponse> MapTags(IEnumerable<Tag> tags) => tags.Select(MapTag).ToList();

        private static TagResponse MapTag(Tag tag) => new()
        {
            Id = tag.Id,
            Name = tag.Name,
            Slug = tag.Slug,
            Description = tag.Description,
            SortOrder = tag.SortOrder,
            IsActive = tag.IsActive,
            HotelCount = tag.HotelTags.Count
        };
    }
}
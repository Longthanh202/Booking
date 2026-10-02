using Booking.Service.Dtos.Tags;
using Booking.Service.Services.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveTags() => Ok(await _tagService.GetActiveTagsAsync());

        [HttpGet("{slug}/hotels")]
        public async Task<IActionResult> GetHotelsByTag(string slug, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 100)
            {
                return BadRequest(new { message = "page phải lớn hơn 0, pageSize từ 1 đến 100." });
            }

            var result = await _tagService.GetHotelsBySlugAsync(slug, page, pageSize);
            return result == null ? NotFound(new { message = "Tag không tồn tại hoặc đã bị ẩn." }) : Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllTags() => Ok(await _tagService.GetAllAsync());

        [Authorize(Roles = "Admin")]
        [HttpPost("admin")]
        public async Task<IActionResult> CreateTag([FromBody] SaveTagRequest request)
        {
            var result = await _tagService.CreateAsync(request);
            return result == null
                ? BadRequest(new { message = "Dữ liệu không hợp lệ hoặc slug đã tồn tại." })
                : CreatedAtAction(nameof(GetHotelsByTag), new { slug = result.Slug }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/{id:guid}")]
        public async Task<IActionResult> UpdateTag(Guid id, [FromBody] SaveTagRequest request)
        {
            var result = await _tagService.UpdateAsync(id, request);
            return result == null
                ? BadRequest(new { message = "Tag không tồn tại, dữ liệu không hợp lệ hoặc slug đã tồn tại." })
                : Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/{id:guid}")]
        public async Task<IActionResult> DeactivateTag(Guid id) =>
            await _tagService.DeactivateAsync(id) ? NoContent() : NotFound();

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/{id:guid}/hotels")]
        public async Task<IActionResult> ReplaceTagHotels(Guid id, [FromBody] SetTagHotelsRequest request)
        {
            return await _tagService.ReplaceHotelsAsync(id, request.HotelIds)
                ? NoContent()
                : BadRequest(new { message = "Tag hoặc một trong các khách sạn không tồn tại." });
        }
    }
}
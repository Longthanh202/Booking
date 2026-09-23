using Booking.Service.Dtos.Reviews;
using Booking.Service.Services.DanhGias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking.Api.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class DanhGiaController : ControllerBase
    {
        private readonly IDanhGiaService _service;

        public DanhGiaController(IDanhGiaService service)
        {
            _service = service;
        }

        [HttpGet("hotel/{hotelId:guid}")]
        public async Task<IActionResult> GetByHotel(Guid hotelId) => Ok(await _service.GetByHotel(hotelId));

        [Authorize(Roles = "Customer")]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            return !Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var customerId)
                ? Unauthorized()
                : Ok(await _service.GetByCustomer(customerId));
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var customerId))
            {
                return Unauthorized();
            }

            var review = await _service.Create(request, customerId);
            return review == null
                ? BadRequest(new { message = "Chỉ có thể đánh giá sau khi hoàn tất booking và dữ liệu phải hợp lệ." })
                : Ok(review);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("owner/hotel/{hotelId:guid}")]
        public async Task<IActionResult> GetOwnerReviews(Guid hotelId)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            var reviews = await _service.GetByHotelForOwner(hotelId, ownerId);
            return reviews == null ? Forbid() : Ok(reviews);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return await _service.Delete(id) ? NoContent() : NotFound();
        }
    }
}
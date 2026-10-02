using Booking.Service.Dtos.Promotions;
using Booking.Service.Services.Promotions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking.Api.Controllers
{
    [Route("api/owner/promotions")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class OwnerPromotionsController : ControllerBase
    {
        private readonly IHotelPromotionService _service;

        public OwnerPromotionsController(IHotelPromotionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Guid hotelId)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            var result = await _service.GetForOwner(ownerId, hotelId);
            return result == null ? Forbid() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] Guid hotelId, [FromBody] SaveHotelPromotionRequest request)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            var result = await _service.Create(ownerId, hotelId, request);
            return result == null
                ? BadRequest(new { message = "Property không thuộc Owner hoặc thông tin promotion/điều kiện không hợp lệ." })
                : CreatedAtAction(nameof(Get), new { hotelId }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SaveHotelPromotionRequest request)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            var result = await _service.Update(ownerId, id, request);
            return result == null
                ? BadRequest(new { message = "Promotion không thuộc Owner hoặc dữ liệu không hợp lệ/trùng code." })
                : Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await _service.Deactivate(ownerId, id) ? NoContent() : NotFound();
        }

        private bool TryGetOwnerId(out Guid ownerId) =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out ownerId);
    }
}
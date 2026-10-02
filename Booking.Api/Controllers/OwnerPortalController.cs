using Booking.Service.Dtos.Owners;
using Booking.Service.Services.Owners;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking.Api.Controllers
{
    [Route("api/owner")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class OwnerPortalController : ControllerBase
    {
        private readonly IOwnerPortalService _service;

        public OwnerPortalController(IOwnerPortalService service)
        {
            _service = service;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard([FromQuery] Guid? hotelId)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            var result = await _service.GetDashboard(ownerId, hotelId);
            return result == null ? Forbid() : Ok(result);
        }

        [HttpGet("reports/revenue")]
        public async Task<IActionResult> GetRevenueReport(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] string groupBy = "day",
            [FromQuery] Guid? hotelId = null)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            var result = await _service.GetRevenueReport(ownerId, hotelId, from, to, groupBy);
            return result == null
                ? BadRequest(new { message = "Khoảng thời gian, khách sạn hoặc groupBy không hợp lệ." })
                : Ok(result);
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetCalendar(
            [FromQuery] Guid hotelId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            var result = await _service.GetCalendar(ownerId, hotelId, from, to);
            return result == null
                ? BadRequest(new { message = "Khách sạn không thuộc Owner hoặc khoảng ngày không hợp lệ." })
                : Ok(result);
        }

        [HttpPost("rooms/{roomId:guid}/availability-blocks")]
        public async Task<IActionResult> CreateRoomBlock(Guid roomId, [FromBody] CreateRoomAvailabilityBlockRequest request)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            var block = await _service.CreateRoomBlock(ownerId, roomId, request);
            return block == null
                ? BadRequest(new { message = "Phòng không thuộc Owner, khoảng thời gian không hợp lệ hoặc bị trùng booking/block." })
                : Ok(block);
        }

        [HttpDelete("availability-blocks/{id:guid}")]
        public async Task<IActionResult> DeleteRoomBlock(Guid id)
        {
            if (!TryGetOwnerId(out var ownerId)) return Unauthorized();
            return await _service.DeleteRoomBlock(ownerId, id) ? NoContent() : NotFound();
        }

        private bool TryGetOwnerId(out Guid ownerId) =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out ownerId);
    }
}
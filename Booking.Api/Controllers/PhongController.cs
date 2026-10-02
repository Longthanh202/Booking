using Booking.Core.Model.Phongs;
using Booking.Data.Repository.Phongs;
using Booking.Service.Dtos.Rooms;
using Booking.Service.Services.GiaPhongs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking.Api.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class PhongController : ControllerBase
    {
        private readonly IPhongService _phongService;
        private readonly IGiaPhongService _giaPhongService;

        public PhongController(IPhongService phongService, IGiaPhongService giaPhongService)
        {
            _phongService = phongService;
            _giaPhongService = giaPhongService;
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> ThemPhong([FromBody] CreateRoomRequest dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            var result = await _phongService.CreateRoom(dto, ownerId);

            if (result != null)
            {
                return Ok(new
                {
                    message = "Thêm phòng thành công"
                });
            }

            return BadRequest(new
            {
                message = "Thêm phòng thất bại"
            });
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("by-room-type/{roomTypeId:guid}")]
        public async Task<IActionResult> GetRooms(Guid roomTypeId)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            return Ok(await _phongService.GetRooms(roomTypeId, ownerId));
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> SuaPhong(Guid id, [FromBody] UpdateRoomRequest dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            var result = await _phongService.UpdateRoom(id, dto, ownerId);
            return result == null ? NotFound(new { message = "Phòng không tồn tại hoặc không thuộc owner." }) : Ok(result);
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> XoaPhong(Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            var deleted = await _phongService.DeleteRoom(id, ownerId);
            return deleted ? NoContent() : BadRequest(new { message = "Không thể xóa phòng đã phát sinh booking hoặc không thuộc owner." });
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("prices/{roomTypeId:guid}")]
        public async Task<IActionResult> GetPrices(Guid roomTypeId)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            return Ok(await _giaPhongService.GetPrices(roomTypeId, ownerId));
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("prices")]
        public async Task<IActionResult> CreatePrice([FromBody] CreateRoomPriceRequest dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            var result = await _giaPhongService.CreatePrice(dto, ownerId);
            return result == null
                ? BadRequest(new { message = "Dữ liệu giá không hợp lệ hoặc loại phòng không thuộc owner." })
                : Ok(result);
        }
    }
}

using Booking.Core.Model.Phongs;
using Booking.Data.Repository.Phongs;
using Booking.Service.Dtos.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class PhongController : ControllerBase
    {
        private readonly IPhongService _phongService;

        public PhongController(IPhongService phongService)
        {
            _phongService = phongService;
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> ThemPhong([FromBody] CreateRoomRequest dto)
        {
            var result = await _phongService.CreateRoom(dto);

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
    }
}

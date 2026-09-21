using Booking.Core.Model.LoaiPhongs;
using Booking.Data.Repository.LoaiPhongs;
using Booking.Service.Dtos.RoomTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Booking.Api.Controllers
{
    [Route("api/room-types")]
    [ApiController]
    public class LoaiPhongController : ControllerBase
    {
        private readonly ILoaiPhongService _loaiPhongService;

        public LoaiPhongController(ILoaiPhongService loaiPhongService)
        {
            _loaiPhongService = loaiPhongService;
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> ThemLoaiPhong([FromBody] List<CreateRoomTypeRequest> dto)
        {
            var result = await _loaiPhongService.CreateRoomTypes(dto);

            if (result != 0)
            {
                return Ok(new
                {
                    message = "Thêm thành công " + result + " loại phòng"
                }); ;
            }

            return BadRequest(new
            {
                message = "Thêm loại phòng thất bại"
            });
        }
        
        [HttpPost("by-hotel")]
        public async Task<IActionResult> GetLoaiPhongByKSID([FromBody] RoomTypeSearchRequest dto)
        {
            var result = await _loaiPhongService.GetRoomTypesByHotelId(dto);
            if (result.IsNullOrEmpty())
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("owner")]
        public async Task<IActionResult> GetOwnerRoomTypes([FromBody] Guid khachSanId)
        {
            var result = await _loaiPhongService.GetOwnerRoomTypes(khachSanId);
            return Ok(result);
        }
    }
}

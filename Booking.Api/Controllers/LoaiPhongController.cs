using Booking.Core.Model.LoaiPhongs;
using Booking.Data.Repository.LoaiPhongs;
using Booking.Service.Dtos.LoaiPhongs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Booking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiPhongController : ControllerBase
    {
        private readonly ILoaiPhongService _loaiPhongService;

        public LoaiPhongController(ILoaiPhongService loaiPhongService)
        {
            _loaiPhongService = loaiPhongService;
        }

        [HttpPost("tao")]
        public async Task<IActionResult> ThemLoaiPhong([FromBody] List<LoaiPhongRequest> dto)
        {
            var result = await _loaiPhongService.TaoLoaiPhong(dto);

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
        
        [HttpPost("get/hotelId")]
        public async Task<IActionResult> GetLoaiPhongByKSID([FromBody] GetLoaiPhongDto dto)
        {
            var result = await _loaiPhongService.GetLoaiPhongByKhachSanId(dto);
            if (result.IsNullOrEmpty())
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPost("owner/loai-phong")]
        public async Task<IActionResult> GetLoaiPhongOwner([FromBody] Guid khachSanId)
        {
            var result = await _loaiPhongService.GetLoaiPhongOwner(khachSanId);
            return Ok(result);
        }
    }
}

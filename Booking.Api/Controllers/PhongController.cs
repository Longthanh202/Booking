using Booking.Core.Model.Phongs;
using Booking.Data.Repository.Phongs;
using Booking.Service.Dtos.Phongs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhongController : ControllerBase
    {
        private readonly IPhongService _phongService;

        public PhongController(IPhongService phongService)
        {
            _phongService = phongService;
        }

        [HttpPost("tao")]
        public async Task<IActionResult> ThemPhong([FromBody] PhongRequest dto)
        {
            var result = await _phongService.TaoPhong(dto);

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

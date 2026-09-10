using Booking.Data.Repository.TienIchs;
using Booking.Service.Dtos.TienIchs;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TienIchController : ControllerBase
    {
        private readonly ITienIchService _tienIchService;

        public TienIchController(ITienIchService tienIchService)
        {
            _tienIchService = tienIchService;
        }

        [HttpPost("tao")]
        public async Task<IActionResult> ThemTienIch([FromBody] List<TienIchRequest> dto)
        {
            var result = await _tienIchService.ThemTienIch(dto);

            if (result != 0)
            {
                return Ok(new
                {
                    message = "Thêm tiện ích thành công"
                });
            }

            return BadRequest(new
            {
                message = "Thêm tiện ích thất bại"
            });
        }
    }
}

using Container_App.Core.Model.TienIchs;
using Container_App.Data.Repository.TienIchs;
using Container_App.Service.Dtos.TienIchs;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> ThemTienIch([FromBody] TienIchRequest dto)
        {
            var result = await _tienIchService.ThemTienIch(dto);

            if (result != null)
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

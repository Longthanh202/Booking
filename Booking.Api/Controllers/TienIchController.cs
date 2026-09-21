using Booking.Data.Repository.TienIchs;
using Booking.Service.Dtos.TienIchs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [Route("api/amenities")]
    [ApiController]
    public class TienIchController : ControllerBase
    {
        private readonly ITienIchService _tienIchService;

        public TienIchController(ITienIchService tienIchService)
        {
            _tienIchService = tienIchService;
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> CreateAmenities([FromBody] List<TienIchRequest> dto)
        {
            var result = await _tienIchService.CreateAmenities(dto);

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

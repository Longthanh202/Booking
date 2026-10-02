using Booking.Data.Repository.TienIchs;
using Booking.Service.Dtos.Amenities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> CreateAmenities([FromBody] List<CreateAmenityRequest> dto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            var result = await _tienIchService.CreateAmenities(dto, ownerId);

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

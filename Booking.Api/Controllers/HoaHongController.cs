using System.Security.Claims;
using Container_App.Data.Repository.Users;
using Container_App.Service.Services.HoaHongs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaHongController : Controller
    {
        private readonly IHoaHongService _hoaHongService;
        private readonly IUserServices _userServices;

        public HoaHongController(IHoaHongService hoaHongService,
            IUserServices userServices)
        {
            _hoaHongService = hoaHongService;
            _userServices = userServices;
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("owner")]
        public async Task<IActionResult> GetHoaHong()
        {
            if (!_userServices.IsAuthenticated())
            {
                return Unauthorized(new { Message = "Vui lòng đăng nhập để xem lịch sử hoa hong" });
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("OwnerId không hợp lệ.");
            }

            var result = await _hoaHongService.LayTheoOwnerId(Guid.Parse(userId));

            return Ok(result);
        }
    }
}
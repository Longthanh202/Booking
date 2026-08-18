using Container_App.Core.Model.DatPhongs;
using Container_App.Data.Repository.DatPhongs;
using Container_App.Data.Repository.Users;
using Container_App.Service.Dtos.DatPhongs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Container_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatPhongController : ControllerBase
    {
        private readonly IDatPhongService _datPhongService;
        private readonly IUserServices _userServices;

        public DatPhongController(IDatPhongService datPhongService, IUserServices userServices)
        {
            _datPhongService = datPhongService;
            _userServices = userServices;
        }

        [HttpPost]
        [Route("tao")]
        public async Task<IActionResult> DatPhong(DatPhongRequest input)
        {
            if (!_userServices.IsAuthenticated())
            {
                return Unauthorized(new { Message = "Vui lòng đăng nhập để đặt phòng" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _datPhongService.DatPhong(input, Guid.Parse(userId));
            if (result != null)
            {
                return Ok(new { Message = "Đặt phòng thành công" });
            }
            else
            {
                return BadRequest(new { Message = "Đặt phòng thất bại" });
            }
        }

        [Authorize(Roles = "Owner")]
        [HttpPut]
        [Route("{id}/check-out")]
        public async Task<IActionResult> Checkout(Guid id)
        {
            var datPhong = await _datPhongService.CapNhatTrangThai(id);
            if (datPhong != null)
            {
                return Ok();
            }

            return BadRequest();
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("owner-bookings")]
        public async Task<IActionResult> GetBookingsOwner([FromBody] DatPhongOwnerRequest dto)
        {
            if (!_userServices.IsAuthenticated())
            {
                return Unauthorized(new { Message = "Vui lòng đăng nhập để đặt phòng" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data = await _datPhongService.GetListBookingOwner(dto, Guid.Parse(userId));
            return Ok(data);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("booking-history")]
        public async Task<IActionResult> BookingHistory(int pageIndex, int pageSize)
        {
            if (!_userServices.IsAuthenticated())
            {
                return Unauthorized(new { Message = "Vui lòng đăng nhập để xem lịch sử đặt phòng" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data = await _datPhongService.BookingHistory(Guid.Parse(userId), pageIndex, pageSize);
            return Ok(data);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("{id}/check-in")]
        public async Task<IActionResult> CheckIn(Guid id)
        {
            try
            {
                await _datPhongService.CheckIn(id);

                return Ok(new
                {
                    status = true,
                    message = "Check-in thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
        [Authorize(Roles = "Owner")]
        [HttpPut("{id}/xacnhan")]
        public async Task<IActionResult> XacNhan(Guid id)
        {
            try
            {
                await _datPhongService.XacNhan(id);

                return Ok(new
                {
                    status = true,
                    message = "Xác nhận Booking thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
    }
}

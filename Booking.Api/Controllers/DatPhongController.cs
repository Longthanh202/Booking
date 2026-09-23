using Booking.Core.Model.DatPhongs;
using Booking.Data.Repository.DatPhongs;
using Booking.Data.Repository.Users;
using Booking.Service.Dtos.Bookings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Booking.Api.Controllers
{
    [Route("api/bookings")]
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
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateBooking(CreateBookingRequest input)
        {
            if (!_userServices.IsAuthenticated())
            {
                return Unauthorized(new { Message = "Vui lòng đăng nhập để đặt phòng" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _datPhongService.CreateBooking(input, Guid.Parse(userId));
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
        public async Task<IActionResult> CheckOut(Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            var datPhong = await _datPhongService.UpdateBookingStatus(id, ownerId);
            if (datPhong != null)
            {
                return Ok();
            }

            return BadRequest();
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("owner")]
        public async Task<IActionResult> GetOwnerBookings([FromBody] OwnerBookingSearchRequest dto)
        {
            if (!_userServices.IsAuthenticated())
            {
                return Unauthorized(new { Message = "Vui lòng đăng nhập để đặt phòng" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data = await _datPhongService.GetOwnerBookings(dto, Guid.Parse(userId));
            return Ok(data);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("history")]
        public async Task<IActionResult> GetBookingHistory(int pageIndex, int pageSize)
        {
            if (!_userServices.IsAuthenticated())
            {
                return Unauthorized(new { Message = "Vui lòng đăng nhập để xem lịch sử đặt phòng" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data = await _datPhongService.GetBookingHistory(Guid.Parse(userId), pageIndex, pageSize);
            return Ok(data);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookingDetail(Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var customerId))
            {
                return Unauthorized();
            }

            var booking = await _datPhongService.GetCustomerBookingById(id, customerId);
            return booking == null ? NotFound() : Ok(booking);
        }

        [Authorize(Roles = "Customer")]
        [HttpPut("{id:guid}/cancel")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var customerId))
            {
                return Unauthorized();
            }

            var cancelled = await _datPhongService.CancelBooking(id, customerId);
            return cancelled
                ? Ok(new { message = "Hủy booking thành công." })
                : BadRequest(new { message = "Booking không tồn tại hoặc không thể hủy ở trạng thái hiện tại." });
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("{id}/check-in")]
        public async Task<IActionResult> CheckIn(Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            await _datPhongService.CheckIn(id, ownerId);

            return Ok(new
            {
                status = true,
                message = "Check-in thành công"
            });
        }
        [Authorize(Roles = "Owner")]
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmBooking(Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            {
                return Unauthorized();
            }

            await _datPhongService.ConfirmBooking(id, ownerId);

            return Ok(new
            {
                status = true,
                message = "Xác nhận Booking thành công"
            });
        }

        [Authorize(Roles ="Owner")]
        [HttpPost("owner/statistics")]
        public async Task<IActionResult> GetOwnerBookingStatistics([FromBody]OwnerBookingStatisticsRequest input)
        {
            if (!_userServices.IsAuthenticated())
            {
                return Unauthorized(new { Message = "Vui lòng đăng nhập" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (input == null)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu thống kê không được để trống"
                });
            }

            var result = await _datPhongService.GetOwnerBookingStatistics(input);

            return Ok(new
            {
                datPhong = result.datPhong,
                tongTien = result.tongTien
            });
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("owner/all")]
        public async Task<IActionResult> GetBookingsByOwner([FromBody] OwnerBookingListRequest dto)
        {
            if (!_userServices.IsAuthenticated())
            {
                return Unauthorized(new { Message = "Vui lòng đăng nhập để đặt phòng" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data = await _datPhongService.GetBookingsByOwner(dto, Guid.Parse(userId));
            return Ok(data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/search")]
        public async Task<IActionResult> GetAdminBookings([FromBody] OwnerBookingListRequest dto)
        {
            return Ok(await _datPhongService.GetAdminBookings(dto));
        }
    }
}

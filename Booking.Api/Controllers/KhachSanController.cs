using Booking.Api.Attributes;
using Booking.Common.Shared;
using Booking.Core.Model.KhachSans;
using Booking.Core.Model.LoaiPhongs;
using Booking.Core.Model.Phongs;
using Booking.Core.Model.TienIchs;
using Booking.Data.Repository.KhachSans;
using Booking.Data.Repository.LoaiPhongs;
using Booking.Data.Repository.Phongs;
using Booking.Data.Repository.TienIchs;
using Booking.Data.Repository.Users;
using Booking.Service.Dtos.Hotels;
using Booking.Service.Services.Cloudinarys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Claims;

namespace Booking.Api.Controllers
{
    [Route("api/hotels")]
    [ApiController]
    public class KhachSanController : Controller
    {
        private readonly IKhachSanService _khachSanService;
       
        public KhachSanController(IKhachSanService khachSanService)
        {
            _khachSanService = khachSanService;
          
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> CreateHotel([FromForm] CreateHotelRequest dto)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            
            var result = await _khachSanService.CreateHotel(dto, Guid.Parse(userId));
            if (!result.Status)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("admin/search")]
        public async Task<IActionResult> AdminGetKhachSans(
            [FromBody] AdminHotelFilterRequest dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Không tìm thấy thông tin tài khoản hoặc phiên đăng nhập không hợp lệ."
                });
            }

            var page = dto.Page <= 0 ? 1 : dto.Page;
            var pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

            var request = new AdminHotelFilterRequest
            {
                Keyword = dto.Keyword,
                ThanhPho = dto.ThanhPho,
                ViDo = dto.ViDo ?? 0,
                KinhDo = dto.KinhDo ?? 0,
                SoSao = dto.SoSao,
                TrangThai = dto.TrangThai,
                Page = page,
                PageSize = pageSize
            };

            var result =
                await _khachSanService.LayDanhSachKhachSanAdminAsync(request);

            return Ok(result);
        }
        
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/search")]
        public async Task<IActionResult> OwnerGetKhachSans(
            [FromBody] OwnerHotelFilterRequest dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Không tìm thấy thông tin tài khoản hoặc phiên đăng nhập không hợp lệ."
                });
            }

            var page = dto.Page <= 0 ? 1 : dto.Page;
            var pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

            var request = new OwnerHotelFilterRequest
            {
                Page = page,
                PageSize = pageSize
            };

            var result =
                await _khachSanService.LayDanhSachKhachSanOwnerAsync(request, Guid.Parse(userIdClaim));

            return Ok(result);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> DetailHotel(Guid id)
        {
            var khachSan = await _khachSanService.GetHotelDetails(id);
            return Ok(khachSan);
        }
        [HttpPost("search")]
        public async Task<IActionResult> FilterHotels([FromBody] HotelFilterRequest dto)
        {
            if (dto.Page <= 0 || dto.PageSize <= 0)
            {
                return BadRequest(new { message = "Số trang (Page) và Kích thước trang (PageSize) phải lớn hơn 0." });
            }
            // Gọi Service xử lý logic; exception được xử lý bởi global middleware.
            var result = await _khachSanService.FilterHotelsAsync(dto);
            return Ok(result);
        }
    }
}

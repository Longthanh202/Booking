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
using Booking.Service.Dtos.KhachSan;
using Booking.Service.Dtos.KhachSanDto;
using Booking.Service.Services.Cloudinarys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Claims;

namespace Booking.Api.Controllers
{
    [Route("api/[controller]")]
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
        [Route("tao")]
        public async Task<IActionResult> TaoKhachSan([FromForm] KhachSanCreateRequest dto)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            
            var result = await _khachSanService.TaoKhachSan(dto, Guid.Parse(userId));
            if (!result.status)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("admin-get")]
        public async Task<IActionResult> AdminGetKhachSans(
            [FromBody] AdminFilterHotelRequestDto dto)
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

            var request = new AdminFilterHotelRequestDto
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
        [HttpPost("owner-get")]
        public async Task<IActionResult> OwnerGetKhachSans(
            [FromBody] OwnerFilterHotelRequestDto dto)
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

            var request = new OwnerFilterHotelRequestDto
            {
                Page = page,
                PageSize = pageSize
            };

            var result =
                await _khachSanService.LayDanhSachKhachSanOwnerAsync(request, Guid.Parse(userIdClaim));

            return Ok(result);
        }
        
        [HttpPost]
        [Route("chitiet")]
        public async Task<IActionResult> DetailHotel([FromBody] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var khachSan = await _khachSanService.DetailKhachSan(Guid.Parse(id));
            return Ok(khachSan);
        }
        [HttpPost]
        [Route("filter")]
        public async Task<IActionResult> FilterHotels([FromBody] FilterHotelRequestDto dto)
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

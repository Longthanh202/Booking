using Container_App.Attributes;
using Container_App.Common.Shared;
using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.Phongs;
using Container_App.Core.Model.TienIchs;
using Container_App.Data.Repository.KhachSans;
using Container_App.Data.Repository.LoaiPhongs;
using Container_App.Data.Repository.Phongs;
using Container_App.Data.Repository.TienIchs;
using Container_App.Data.Repository.Users;
using Container_App.Service.Dtos.KhachSan;
using Container_App.Service.Dtos.KhachSanDto;
using Container_App.Service.Services.Cloudinarys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Claims;

namespace Container_App.Controllers
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

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest();
                }
                var khachSan = await _khachSanService.DetailKhachSan(Guid.Parse(id));
                return Ok(khachSan);

            }
            catch (SqlException ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.Error.WriteLine($"SQL Exception: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.Error.WriteLine($"General Exception: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpPost]
        [Route("filter")]
        public async Task<IActionResult> FilterHotels([FromBody] FilterHotelRequestDto dto)
        {
            if (dto.Page <= 0 || dto.PageSize <= 0)
            {
                return BadRequest(new { message = "Số trang (Page) và Kích thước trang (PageSize) phải lớn hơn 0." });
            }
            try
            {
                // 2. Gọi Service xử lý logic
                var result = await _khachSanService.FilterHotelsAsync(dto);

                // 3. Trả về kết quả HTTP 200 OK
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());

                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.", error = ex.Message.ToString() });
            }
        }
    }
}

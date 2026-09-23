using Booking.Core.Model.Banners;
using Booking.Data.Repository.Banners;
using Booking.Service.Dtos.Banners;
using Booking.Service.Services.Cloudinarys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [Route("api/banners")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBanner([FromForm] CreateBannerRequest file)
        {
            var banner = await _bannerService.CreateBanner(file);
            return Ok(banner);
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> GetActiveBanners()
        {
            var result = await _bannerService.GetActiveBanners();
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBanners(string? keyword, int isActive = 0, int startRow = 0, int endRow = 50)
        {
            if (startRow < 0 || endRow <= startRow)
            {
                return BadRequest(new { message = "Khoảng phân trang không hợp lệ." });
            }

            return Ok(await _bannerService.GetBanners(keyword ?? string.Empty, isActive, startRow, endRow));
        }

        [HttpPut("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBanner(long id, [FromBody] UpdateBannerRequest request)
        {
            var result = await _bannerService.UpdateBanner(id, request);
            return result == null ? BadRequest(new { message = "Banner không tồn tại hoặc dữ liệu không hợp lệ." }) : Ok(result);
        }

        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBanner(long id)
        {
            return await _bannerService.DeleteBanner(id) ? NoContent() : NotFound();
        }
    }
}

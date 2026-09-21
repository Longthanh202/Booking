using Booking.Core.Model.Banners;
using Booking.Data.Repository.Banners;
using Booking.Service.Dtos.Banner;
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
        public async Task<IActionResult> CreateBanner([FromForm] InsertBannerDto file)
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
    }
}

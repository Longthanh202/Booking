using Booking.Core.Model.Banners;
using Booking.Data.Repository.Banners;
using Booking.Service.Dtos.Banner;
using Booking.Service.Services.Cloudinarys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpPost]
        [Route("tao")]
        public async Task<IActionResult> InsertBanner([FromForm] InsertBannerDto file)
        {
            var banner = await _bannerService.InsertBanner(file);
            return Ok(banner);
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetBannerIsActive()
        {
            var result = await _bannerService.GetBannerIsActive();
            return Ok(result);
        }
    }
}

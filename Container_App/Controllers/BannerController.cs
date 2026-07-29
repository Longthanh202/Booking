using Container_App.Core.Model.Banners;
using Container_App.Data.Repository.Banners;
using Container_App.Service.Dtos.Banner;
using Container_App.Service.Services.Cloudinarys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Container_App.Controllers
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

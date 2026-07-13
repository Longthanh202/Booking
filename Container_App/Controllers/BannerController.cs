using Container_App.Core.Interface.Banners;
using Container_App.Core.Model.Banners;
using Container_App.Model.Bannners;
using Container_App.Service.Services.Cloudinarys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Container_App.Controllers
{
    [ApiController]
    [Route("api")]
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        private readonly CloudinaryService _cloudinaryService;
        public BannerController(IBannerService bannerService, CloudinaryService cloudinaryService)
        {
            _bannerService = bannerService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost]
        [Route("admin/insert-banner")]
        public async Task<IActionResult> InsertBanner([FromForm] InsertBannerDto file)
        {
            var url = await _cloudinaryService.UploadImageAsync(file.File);
            var banner = new Banner
            {
                Title = file.Title,
                Subtitle = file.Subtitle,
                IsActive = file.IsActive,
                Url = url
            };
            var result = await _bannerService.InsertBanner(banner);
            if (result != -1)
            {
                return Ok(new { message = "Banner inserted successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to insert banner" });
            }
        }

        [HttpGet]
        [Route("client/get-banners")]
        public async Task<IActionResult> GetBannerIsActive()
        {
            var result = await _bannerService.GetBannerIsActive();
            return Ok(result);
        }
    }
}

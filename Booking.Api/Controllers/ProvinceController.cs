using Booking.Data.Repository.Provinces;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        private readonly IProvinceService _provinceService;
        public ProvinceController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetProvinces()
        {
            var result = await _provinceService.GetProvinces();
            return Ok(result);
        }
    }
}

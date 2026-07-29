using Container_App.Data.Repository.Provinces;
using Microsoft.AspNetCore.Mvc;

namespace Container_App.Controllers
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

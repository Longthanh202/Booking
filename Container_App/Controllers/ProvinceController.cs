using Container_App.Data.Repository.Provinces;
using Microsoft.AspNetCore.Mvc;

namespace Container_App.Controllers
{
    public class ProvinceController : Controller
    {
        private readonly IProvinceService _provinceService;
        public ProvinceController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }
        [HttpGet]
        [Route("api/provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var result = await _provinceService.GetProvinces();
            return Ok(result);
        }
    }
}

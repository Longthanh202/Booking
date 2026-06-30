using Container_App.Common.Shared;
using Container_App.Core.Interface.KhachSans;
using Container_App.Core.Model.KhachSans;
using Container_App.Model.KhachSans;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Text;

namespace Container_App.Controllers
{
    [Route("api/client")]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IKhachSanService _khachSanService;
        private readonly IKhachSanImageService _khachSanImageService;
        const int PAGE_SIZE = 10;
        public HomeController(IConfiguration configuration, IKhachSanService khachSanService,
            IKhachSanImageService khachSanImageService)
        {
            _configuration = configuration;
            _khachSanService = khachSanService;
            _khachSanImageService = khachSanImageService;
        }
        [HttpGet]
        [Route("view")]
        public IActionResult Home()
        {
            return View();
        }

        [HttpPost]
        [Route("khachsans/filter")]
        public async Task<IActionResult> FilterHotels([FromBody] FilterHotelsDto dto)
        {

            try
            {
                int startRow = Paginations.GetStartRow(dto.Page, PAGE_SIZE);
                int endRow = Paginations.GetEndRow(dto.Page, PAGE_SIZE);

                var khachSans = Enumerable.Empty<KhachSan>();

                khachSans = await _khachSanService.FilterHotels(
                dto.Keyword, dto.ProvinceCode, dto.SoKhach, dto.NgayNhanPhong, dto.NgayTraPhong, startRow, endRow);

                var hotelIds = khachSans
                .Select(x => x.Id)
                .Distinct()
                .ToList();
                var hotelImages = await _khachSanImageService.GetHotelImages(hotelIds);

                // MAP IMAGES
                var imageLookup = hotelImages
                    .GroupBy(x => x.KhachSanId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToList()
                    );

                foreach (var hotel in khachSans)
                {
                    hotel.Urls = imageLookup.ContainsKey(hotel.Id)
                        ? imageLookup[hotel.Id]
                            .Select(x => x.Url)
                            .ToList()
                        : new List<string>();
                }
                int totalRow = khachSans.FirstOrDefault()?.TotalRow ?? 0;
                int totalPage = Paginations.GetTotalPages(totalRow, PAGE_SIZE);
                return Ok(new { Data = khachSans, TotalPage = totalPage });
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
    }
}

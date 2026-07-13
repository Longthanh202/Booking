using Container_App.Common.Shared;
using Container_App.Core.Interface.KhachSans;
using Container_App.Core.Interface.LoaiPhongs;
using Container_App.Core.Interface.Redis;
using Container_App.Core.Interface.TienIchs;
using Container_App.Core.Model.KhachSans;
using Container_App.Model.KhachSans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Data.SqlClient;
using System.Diagnostics;
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
        private readonly ILoaiPhongService _loaiPhongService;
        private readonly ITienIchService _tienIchService;
        private readonly IRedisService _redisService;
        const int PAGE_SIZE = 10;
        public HomeController(IConfiguration configuration, IKhachSanService khachSanService,
            IKhachSanImageService khachSanImageService, ILoaiPhongService loaiPhongService, ITienIchService tienIchService,
            IRedisService redisService)
        {
            _configuration = configuration;
            _khachSanService = khachSanService;
            _khachSanImageService = khachSanImageService;
            _loaiPhongService = loaiPhongService;
            _tienIchService = tienIchService;
            _redisService = redisService;
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
            var stopwatch = Stopwatch.StartNew();

            try
            {
                string cacheKey =
               $"hotel-filter:" +
               $"{dto.Keyword ?? ""}:" +
               $"{dto.ProvinceCode?.ToString() ?? ""}:" +
               $"{dto.SoKhach?.ToString() ?? ""}:" +
               $"{dto.NgayNhanPhong?.ToString("yyyyMMdd") ?? ""}:" +
               $"{dto.NgayTraPhong?.ToString("yyyyMMdd") ?? ""}:" +
               $"{dto.Page}";

                var cache = await _redisService.GetObject<FilterHotelResponse>(cacheKey);

                if (cache != null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Load Redis: {stopwatch.ElapsedMilliseconds} ms");

                    return Ok(cache);
                }
                int startRow = Paginations.GetStartRow(dto.Page, PAGE_SIZE);
                int endRow = Paginations.GetEndRow(dto.Page, PAGE_SIZE);

                var khachSans = await _khachSanService.FilterHotels(
                    dto.Keyword,
                    dto.ProvinceCode,
                    dto.SoKhach,
                    dto.NgayNhanPhong,
                    dto.NgayTraPhong,
                    startRow,
                    endRow);

                var hotelIds = khachSans
                    .Select(x => x.Id)
                    .Distinct()
                    .ToList();

                var hotelImages = await _khachSanImageService.GetHotelImages(hotelIds);

                var imageLookup = hotelImages
                    .GroupBy(x => x.KhachSanId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToList());

                foreach (var hotel in khachSans)
                {
                    hotel.Urls = imageLookup.ContainsKey(hotel.Id)
                        ? imageLookup[hotel.Id].Select(x => x.Url).ToList()
                        : new List<string>();
                }

                int totalRow = khachSans.FirstOrDefault()?.TotalRow ?? 0;
                int totalPage = Paginations.GetTotalPages(totalRow, PAGE_SIZE);

                var result = new FilterHotelResponse
                {
                    Data = khachSans.ToList(),
                    TotalPage = totalPage
                };
                await _redisService.SetObject(cacheKey, result, TimeSpan.FromSeconds(30));

                stopwatch.Stop();
                Console.WriteLine($"FilterHotels API: {stopwatch.ElapsedMilliseconds} ms");
                return Ok(result);
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("khachsans/detail")]
        public async Task<IActionResult> DetailHotel([FromBody] string id)
        {

            try
            {
                if (id.IsNullOrEmpty())
                {
                    return BadRequest();
                }
                Guid IdFormat = Guid.Parse(id);
                var khachSan = await _khachSanService.DetailKhachSan(IdFormat);
                if (khachSan == null)
                {
                    return StatusCode(500, "Server Error");
                }
                var loaiPhongs = await _loaiPhongService.GetLoaiPhongByKhachSanId(IdFormat);
                var tienIchs = await _tienIchService.GetTienIchKhachSanByKhachSanId(IdFormat);
                var images = await _khachSanImageService.GetListImageByKhachSanId(IdFormat);
                return Ok(new DetailHotel
                {
                    KhachSan = khachSan,
                    tienIchs = tienIchs.ToList(),
                    loaiPhongs = loaiPhongs.ToList(),
                    KhachSanImages = images.ToList(),
                });

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

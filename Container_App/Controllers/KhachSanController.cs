using Container_App.Attributes;
using Container_App.Common.Shared;
using Container_App.Core.Interface.KhachSans;
using Container_App.Core.Interface.LoaiPhongs;
using Container_App.Core.Interface.Phongs;
using Container_App.Core.Interface.TienIchs;
using Container_App.Core.Interface.Users;
using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.Phongs;
using Container_App.Core.Model.TienIchs;
using Container_App.Model.KhachSans;
using Container_App.Service.Services.Cloudinarys;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Container_App.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class KhachSanController : Controller
    {
        private readonly IKhachSanService _khachSanService;
        private readonly ITienIchService _tienIchService;
        private readonly ILoaiPhongService _loaiPhongService;
        private readonly IPhongService _phongService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IUserServices _userService;

        const int PAGE_SIZE = 10;
        public KhachSanController(IKhachSanService khachSanService, ITienIchService tienIchService,
            ILoaiPhongService loaiPhongService, IPhongService phongService, CloudinaryService cloudinaryService, 
            IUserServices userService)
        {
            _khachSanService = khachSanService;
            _tienIchService = tienIchService;
            _loaiPhongService = loaiPhongService;
            _phongService = phongService;
            _cloudinaryService = cloudinaryService;
            _userService = userService;
        }

        [HasPermission("khachsan", "insert")]
        [HttpPost]
        [Route("khachsan/tao")]
        public async Task<IActionResult> TaoKhachSan([FromForm] KhachSanCreateDto dto)
        {
            if (!TimeSpan.TryParse(dto.GioNhanPhong, out var gioNhan))
                return BadRequest("Giờ nhận phòng không hợp lệ");

            if (!TimeSpan.TryParse(dto.GioTraPhong, out var gioTra))
                return BadRequest("Giờ trả phòng không hợp lệ");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var images = new List<string>();
            foreach (var file in dto.Files)
            {
                var url = await _cloudinaryService.UploadImageAsync(file);
                images.Add(url);
            }

            var KhachSan = new KhachSan
            {
                TenKhachSan = dto.TenKhachSan,
                MoTa = dto.MoTa,
                DiaChi = dto.DiaChi,
                ThanhPho = dto.ThanhPho,
                ViDo = dto.ViDo,
                KinhDo = dto.KinhDo,
                SoSao = dto.SoSao,
                GioNhanPhong = dto.GioNhanPhong,
                GioTraPhong = dto.GioTraPhong,
                TrangThai = dto.TrangThai,
                NguoiTao = Guid.Parse(userId),
                Urls = images
            };           
            int insert = await _khachSanService.TaoKhachSan(KhachSan);
            if (insert != -1)
            {
                return Ok(new { Message = "Tạo khách sạn thành công" });
            }
            return BadRequest(new { Message = "Tạo khách sạn thất bại" });
        }

        [HttpPost]
        [Route("tienich/tao")]
        public async Task<IActionResult> ThemTienIch([FromBody] TienIch dto)
        {

            int insert = await _tienIchService.ThemTienIch(dto);
            if (insert != -1)
            {
                return Ok(new { Message = "Thêm tiện ích thành công" });
            }
            return BadRequest(new { Message = "Thêm tiện ích thất bại" });
        }

        [HttpPost]
        [Route("loaiphong/tao")]
        public async Task<IActionResult> ThemLoaiPhong([FromBody] LoaiPhong dto)
        {
            if (!_userService.IsAuthenticated())
            {
                return Unauthorized(new
                {
                    message = "Vui lòng đăng nhập"
                });
            }
            int insert = await _loaiPhongService.TaoLoaiPhong(dto);
            if (insert != -1)
            {
                return Ok(new { Message = "Thêm loại phòng thành công" });
            }
            return BadRequest(new { Message = "Thêm loại phòng thất bại" });
        }

        [HttpPost]
        [Route("phong/tao")]
        public async Task<IActionResult> ThemPhong([FromBody] Phong dto)
        {
            int insert = await _phongService.TaoPhong(dto);
            if (insert != -1)
            {
                return Ok(new { Message = "Thêm phòng thành công" });
            }
            return BadRequest(new { Message = "Thêm phòng thất bại" });
        }

        [HasPermission("khachsan", "view")]
        [HttpPost]
        [Route("khachsans/get")]
        public async Task<IActionResult> GetKhachSans([FromBody] KhachSanFilterDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            int startRow = Paginations.GetStartRow(dto.Page, PAGE_SIZE);
            int endRow = Paginations.GetEndRow(dto.Page, PAGE_SIZE);

            var khachSans = Enumerable.Empty<KhachSan>();

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (User.FindFirst(ClaimTypes.Role)?.Value == "Admin")
            {
                khachSans = await _khachSanService.LayDanhSachKhachSanAdmin(
                dto.Keyword, dto.ThanhPho, dto.ViDo ?? 0, dto.KinhDo ?? 0,
                dto.SoSao, dto.TrangThai, startRow, endRow);
            }
            else
            {
               khachSans = await _khachSanService.LayDanhSachKhachSanOwner(
               dto.Keyword, dto.ThanhPho, dto.ViDo ?? 0, dto.KinhDo ?? 0,
               dto.SoSao, dto.TrangThai, Guid.Parse(userId), startRow, endRow);
            }

            int totalRow = khachSans.FirstOrDefault()?.TotalRow ?? 0;
            int totalPage = Paginations.GetTotalPages(totalRow, PAGE_SIZE);
            return Ok(new { Data = khachSans, TotalPage = totalPage });
        }
    }
}

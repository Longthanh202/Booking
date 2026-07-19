using Container_App.Attributes;
using Container_App.Common.Shared;
using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.Phongs;
using Container_App.Core.Model.TienIchs;
using Container_App.Data.Repository.KhachSans;
using Container_App.Data.Repository.LoaiPhongs;
using Container_App.Data.Repository.Phongs;
using Container_App.Data.Repository.TienIchs;
using Container_App.Data.Repository.Users;
using Container_App.Model.KhachSans;
using Container_App.Service.Dtos.KhachSan;
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
        public async Task<IActionResult> TaoKhachSan([FromForm] KhachSanCreateRequest dto)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            
            var result = await _khachSanService.TaoKhachSan(dto, Guid.Parse(userId));
            if (!result.status)
            {
                return BadRequest();
            }
            return Ok(result);
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

            //int totalRow = khachSans.FirstOrDefault()?.TotalRow ?? 0;
            //int totalPage = Paginations.GetTotalPages(totalRow, PAGE_SIZE);
            return Ok(new { Data = khachSans, 
                //TotalPage = totalPage 
            });
        }


    }
}

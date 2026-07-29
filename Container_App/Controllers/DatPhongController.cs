using Container_App.Core.Model.DatPhongs;
using Container_App.Data.Repository.DatPhongs;
using Container_App.Data.Repository.Users;
using Container_App.Service.Dtos.DatPhongs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Container_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatPhongController : ControllerBase
    {
        private readonly IDatPhongService _datPhongService;
        private readonly IUserServices _userServices;

        public DatPhongController(IDatPhongService datPhongService, IUserServices userServices)
        {
            _datPhongService = datPhongService;
            _userServices = userServices;
        }

        [HttpPost]
        [Route("tao")]
        public async Task<IActionResult> DatPhong(DatPhongRequest input)
        {
            //if(!_userServices.IsAuthenticated())
            //{
            //    return Unauthorized(new { Message = "Vui lòng đăng nhập để đặt phòng" });
            //}
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = "3E32A4A2-DB9E-45D8-B12A-5231EDE579C8";
            var result = await _datPhongService.DatPhong(input, Guid.Parse(userId));
            if (result != null)
            {
                return Ok(new { Message = "Đặt phòng thành công" });
            }
            else
            {
                return BadRequest(new { Message = "Đặt phòng thất bại" });
            }
        }
        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Checkout(Guid input)
        {
            var datPhong = await _datPhongService.CapNhatTrangThai(input);
            if (datPhong != null) 
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}

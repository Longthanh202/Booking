using Container_App.Core.Interface.DatPhongs;
using Container_App.Core.Interface.Users;
using Container_App.Core.Model.DatPhongs;
using Container_App.Model.DatPhongs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Container_App.Controllers
{
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
        [Route("api/datphong")]
        public async Task<IActionResult> DatPhong(DatPhongRequest input)
        {
            if(!_userServices.IsAuthenticated())
            {
                return Unauthorized(new { Message = "Vui lòng đăng nhập để đặt phòng" });
            }
            var result = await _datPhongService.DatPhong(input.DatPhong, input.ChiTietDatPhongs);
            if (result != null)
            {
                return Ok(new { Message = "Đặt phòng thành công", MaDatPhong = result });
            }
            else
            {
                return BadRequest(new { Message = "Đặt phòng thất bại" });
            }
        }
    }
}

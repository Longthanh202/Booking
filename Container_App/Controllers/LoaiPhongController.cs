using Container_App.Core.Model.LoaiPhongs;
using Container_App.Data.Repository.LoaiPhongs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Container_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiPhongController : ControllerBase
    {
        private readonly ILoaiPhongService _loaiPhongService;

        public LoaiPhongController(ILoaiPhongService loaiPhongService)
        {
            _loaiPhongService = loaiPhongService;
        }

        [HttpPost("tao")]
        public async Task<IActionResult> ThemLoaiPhong([FromBody] LoaiPhong dto)
        {
            var result = await _loaiPhongService.TaoLoaiPhong(dto);

            if (result != null)
            {
                return Ok(new
                {
                    message = "Thêm loại phòng thành công"
                });
            }

            return BadRequest(new
            {
                message = "Thêm loại phòng thất bại"
            });
        }
    }
}

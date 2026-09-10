using Booking.Common.Shared;
using Booking.Core.Model.KhachSans;
using Booking.Data.Repository.KhachSans;
using Booking.Data.Repository.LoaiPhongs;
using Booking.Data.Repository.Redis;
using Booking.Data.Repository.TienIchs;
using Booking.Service.Services.KhachSanImage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace Booking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        
        public HomeController()
        {
            
        }
        [HttpGet]
        [Route("view")]
        public IActionResult Home()
        {
            return Ok();
        }  
    }
}

//chưa tạo class Dto cho các chức năng tạo loại phòng, tiện ích, phòng


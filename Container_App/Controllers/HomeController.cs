using Container_App.Common.Shared;
using Container_App.Core.Model.KhachSans;
using Container_App.Data.Repository.KhachSans;
using Container_App.Data.Repository.LoaiPhongs;
using Container_App.Data.Repository.Redis;
using Container_App.Data.Repository.TienIchs;
using Container_App.Service.Services.KhachSanImage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace Container_App.Controllers
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


//Aa123456@

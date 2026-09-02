
using Booking.Core.Model.Email;
using Booking.Core.Model.Permissions;
using Booking.Core.Model.RefreshTokens;
using Booking.Core.Model.Users;
using Booking.Data.Repository.Emails;
using Booking.Data.Repository.Permissions;
using Booking.Data.Repository.RabbitMQ;
using Booking.Data.Repository.RefreshTokens;
using Booking.Data.Repository.RolePermissions;
using Booking.Data.Repository.Users;
using Booking.Service.Dtos.Login;
using Booking.Service.Dtos.RolePermission;
using Booking.Service.Dtos.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.Design;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Booking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IRefreshTokenService _refreshTokenService;
        public UserController(IUserServices userServices,
            IRolePermissionService rolePermissionService,
            IRefreshTokenService refreshTokenService)
        {
            _userServices = userServices;                
            _rolePermissionService = rolePermissionService;
            _refreshTokenService = refreshTokenService;
           
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginResquest dto)
        {
            var result = await _userServices.Login(dto);

            if (!result.status)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> GetProfile()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleId = User.FindFirst("RoleId")?.Value;

            if (userId == null || roleId == null)
            {
                return Unauthorized();
            }

            var result = await _userServices.GetById(Guid.Parse(userId), Guid.Parse(roleId));

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            string accessToken = await _refreshTokenService.RefreshToken();
            if(string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }
            return Ok(accessToken);
        }

        [HttpPost]
        [Route("insert-role-permission")]
        public async Task<IActionResult> InsertRolePermission([FromBody] RolePermissionRequset dto)
        {
            await _rolePermissionService.Insert(dto);
            return Ok();
        }       

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRequset u)
        {
            var result =  await _userServices.Register(u);
            if(result != null)
            {
                return Ok(u);
            }
            return BadRequest();
        }
        
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new
                {
                    message = "Vui lòng nhập username."
                });
            }

            await _userServices.QuenMatKhau(username);

            return Ok(new
            {
                message = "Nếu tài khoản tồn tại, mã xác nhận đã được gửi đến email."
            });
        }
    }
}

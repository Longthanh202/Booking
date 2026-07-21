using Container_App.Attributes;
using Container_App.Core.Model.Email;
using Container_App.Core.Model.Permissions;
using Container_App.Core.Model.RefreshTokens;
using Container_App.Core.Model.Users;
using Container_App.Data.Repository.Emails;
using Container_App.Data.Repository.Permissions;
using Container_App.Data.Repository.RabbitMQ;
using Container_App.Data.Repository.RefreshTokens;
using Container_App.Data.Repository.RolePermissions;
using Container_App.Data.Repository.Users;
using Container_App.Service.Dtos.Login;
using Container_App.Service.Dtos.RolePermission;
using Container_App.Service.Dtos.UserProfile;
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

namespace Container_App.Controllers
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
            var roleId = User.FindFirst(ClaimTypes.Role)?.Value;

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
            await _userServices.Register(u);
            return Ok(u);
        }
    }
}

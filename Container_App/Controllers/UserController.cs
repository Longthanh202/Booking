using Container_App.Attributes;
using Container_App.Core.Interface.Emails;
using Container_App.Core.Interface.Permissions;
using Container_App.Core.Interface.RabbitMQ;
using Container_App.Core.Interface.RefreshTokens;
using Container_App.Core.Interface.RolePermissions;
using Container_App.Core.Interface.Users;
using Container_App.Core.Model.Email;
using Container_App.Core.Model.Permissions;
using Container_App.Core.Model.RefreshTokens;
using Container_App.Core.Model.Users;
using Container_App.Model.Emails;
using Container_App.Model.RolePermissions;
using Container_App.Model.Users;
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
    [ApiController]
    public class UserController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IUserServices _userServices;
        private readonly IPermissionService _permissionServices;
        private readonly IMemoryCache _memoryCache;
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IEmailService _emailService;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;
        public UserController(IUserServices userServices, IPermissionService permissionService,
            IConfiguration config, IMemoryCache memoryCache, IRolePermissionService rolePermissionService,
            IRefreshTokenService refreshTokenService, IEmailService emailService, IRabbitMQPublisher rabbitMQPublisher)
        {
            _userServices = userServices;
            _permissionServices = permissionService;
            _config = config;
            _memoryCache = memoryCache;
            _emailService = emailService;
            _rolePermissionService = rolePermissionService;
            _refreshTokenService = refreshTokenService;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        [HttpPost]
        [Route("api/login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrEmpty(dto.username) || string.IsNullOrEmpty(dto.password))
            {
                return BadRequest(new { status = false, message = "Username hoặc Password không được để trống" });
            }

            var user = await _userServices.Login(dto.username, dto.password);
            if (user == null)
            {
                return Unauthorized(new { status = false, message = "Tên đăng nhập hoặc mật khẩu không chính xác" });
            }
            var token = GenerateToken(user);
            var refreshToken = GenerateRefreshToken();

            await _refreshTokenService.InsertRefreshToken(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpireDate = 30,
                FullName = user.FullName,
                RoleId = user.RoleId,
                RoleName = user.RoleName,
            });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(30)
            });

            return Ok(new
            {
                status = true,
                token = token,
                message = "Đăng nhập thành công"
            });
        }

        [HttpGet]
        [Route("api/me")]
        public async Task<IActionResult> GetProfile()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleId = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == null || roleId == null)
            {
                return Unauthorized();
            }

            var cacheKey = $"Permission_Role_{roleId}";
            List<string> permissionKeys;

            if (!_memoryCache.TryGetValue(cacheKey, out permissionKeys))
            {
                var permissions = (await _permissionServices.GetListPermissionByUser(Guid.Parse(userId))).ToList();

                permissionKeys = permissions
                    .Select(p => $"{p.ResourceName.ToLower()}_{p.Action.ToLower()}")
                    .ToList();

                _memoryCache.Set(cacheKey, permissionKeys, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                });
            }

            var user = await _userServices.GetById(Guid.Parse(userId));

            return Ok(new
            {
                fullName = user.FullName,
                permissions = permissionKeys
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return Unauthorized();

            var existingToken = await _refreshTokenService.CheckStatusefreshToken(refreshToken);
            if(existingToken == null)
                return Unauthorized();

            var newAccessToken = GenerateToken(new UserProfile
            {
                Id = existingToken.UserId,
                FullName = existingToken.FullName,
                RoleId = existingToken.RoleId,
                RoleName = existingToken.RoleName
            });
        
            return Ok(new { token = newAccessToken });
        }

        [HasPermission("user", "insert")]
        [HttpPost]
        [Route("api/insert-user")]
        public async Task<IActionResult> Insert([FromBody] UserProfile u)
        {
            int result = await _userServices.Insert(u);
            if (result == 0)
            {
                return BadRequest();
            }
            if (result != -1)
            {
                return Json(new { status = true, message = "Thêm tài khoản thành công" });
            }
            return Json(new { status = false, message = "Thêm tài khoản thất bại" });
        }

        [HttpPost]
        [Route("api/insert-role-permission")]
        public async Task<IActionResult> InsertRolePermission([FromBody] CreateRolePermissionDto dto)
        {
            var result = await _rolePermissionService.Insert(dto.RoleId, dto.RolePermissions);
            if (result == 0)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        private string GenerateToken(UserProfile user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var jwtKey = jwtSection["Key"];
            var expireMinutes = jwtSection["ExpireMinutes"];

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("FullName", user.FullName.ToString()),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim(ClaimTypes.Role, user.RoleName.ToString())
            };

            var key = new SymmetricSecurityKey(Convert.FromBase64String(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(expireMinutes)
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        [HttpPost]
        [Route("api/register")]
        public async Task<IActionResult> Register([FromBody] UserProfile u)
        {
            int result = await _userServices.Insert(u);

            if (result <= 0)
            {
                return BadRequest();
            }

            await _rabbitMQPublisher.PublishAsync(
                "email_queue",
                new SendEmailEvent
                {
                    ToEmail = u.Email,
                    Subject =
                        "Đăng ký tài khoản thành công",

                    Body =
                        $"Xin chào {u.FullName}"
                });

            return Ok(new
            {
                status = true,
                message =
                    "Đăng ký thành công"
            });
        }
    }
}

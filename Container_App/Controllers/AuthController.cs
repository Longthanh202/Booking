using Container_App.Service.Dtos.GoogleLogin;
using Container_App.Service.Services.Auths;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Container_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
            {
                return BadRequest(new { message = "IdToken không được để trống!" });
            }

            try
            {
                // Lấy Default RoleId (VD: Role Khách hàng) từ appsettings.json
                Guid defaultRoleId = Guid.Parse(_config["AppSettings:DefaultCustomerRoleId"]!);

                var response = await _authService.LoginWithGoogleAsync(request.IdToken, defaultRoleId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}


using Booking.Api.Middleware.RateLimit;
using Booking.Core.Model.Email;
using Booking.Core.Model.RefreshTokens;
using Booking.Core.Model.Users;
using Booking.Data.Repository.Emails;
using Booking.Data.Repository.RabbitMQ;
using Booking.Data.Repository.RefreshTokens;
using Booking.Data.Repository.Users;
using Booking.Service.Dtos.Authentication;
using Booking.Service.Dtos.Users;
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
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRateLimiter _rateLimiter;
        public UserController(IUserServices userServices,
            IRefreshTokenService refreshTokenService,
            IRateLimiter rateLimiter)
        {
            _userServices = userServices;                
            _refreshTokenService = refreshTokenService;
            _rateLimiter = rateLimiter;
           
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var rate = _rateLimiter.CheckAsync(
                    key: $"login:{dto.Username}",
                    limit: 5,
                    window: TimeSpan.FromMinutes(15),
                    algorithm: RateLimitAlgorithm.FixedWindow
                );
            if (rate.IsCompleted)
            {
                return StatusCode(429, $"Thử lại sau vài giây.");
            }
            var result = await _userServices.Login(dto);

            if (!result.Status)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        [Route("me")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleId = User.FindFirst("RoleId")?.Value;

            if (userId == null || roleId == null)
            {
                return Unauthorized();
            }

            var result = await _userServices.GetUserProfileById(Guid.Parse(userId), Guid.Parse(roleId));

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

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminUsers(
            [FromQuery] string? keyword,
            [FromQuery] int? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 100 || (status.HasValue && status is not (0 or 1)))
            {
                return BadRequest(new { message = "Tham số lọc hoặc phân trang không hợp lệ." });
            }

            return Ok(await _userServices.GetAdminUsers(keyword, status, page, pageSize));
        }

        [HttpPut("admin/{userId:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAccountStatus(Guid userId, [FromBody] UpdateAccountStatusRequest request)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var currentUserId))
            {
                return Unauthorized();
            }

            if (userId == currentUserId && request.IsDel == 1)
            {
                return BadRequest(new { message = "Không thể khóa tài khoản Admin đang đăng nhập." });
            }

            if (!await _userServices.SetAccountStatus(userId, request.IsDel))
            {
                return BadRequest(new
                {
                    message = "Không tìm thấy tài khoản hoặc không thể mở khóa owner chưa được duyệt."
                });
            }

            return Ok(new { message = request.IsDel == 0 ? "Đã mở khóa tài khoản." : "Đã khóa tài khoản." });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest u)
        {
            var result =  await _userServices.Register(u);
            if(result != null)
            {
                return Ok(new { message = "Đăng ký tài khoản thành công." });
            }
            return BadRequest();
        }

        [HttpPost("register-owner")]
        public async Task<IActionResult> RegisterOwner([FromBody] OwnerRegistrationRequest request)
        {
            if (!await _userServices.RegisterOwner(request))
            {
                return Conflict(new { message = "Username hoặc email đã được sử dụng." });
            }

            return Accepted(new
            {
                message = "Đăng ký Owner thành công. Tài khoản đang chờ Admin duyệt."
            });
        }

        [HttpGet("admin/owner-registrations")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOwnerRegistrations(
            [FromQuery] int? status,
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 100 ||
                (status.HasValue && status is not (0 or 1 or 2)))
            {
                return BadRequest(new { message = "Tham số lọc hoặc phân trang không hợp lệ." });
            }

            return Ok(await _userServices.GetOwnerRegistrations(status, keyword, page, pageSize));
        }

        [HttpPut("admin/owner-registrations/{userId:guid}/approve")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> ApproveOwnerRegistration(Guid userId) =>
            ReviewOwnerRegistration(userId, approved: true, reason: null);

        [HttpPut("admin/owner-registrations/{userId:guid}/reject")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> RejectOwnerRegistration(
            Guid userId, [FromBody] RejectOwnerRegistrationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return Task.FromResult<IActionResult>(
                    BadRequest(new { message = "Vui lòng nhập lý do từ chối." }));
            }

            return ReviewOwnerRegistration(userId, approved: false, reason: request.Reason.Trim());
        }

        private async Task<IActionResult> ReviewOwnerRegistration(
            Guid userId, bool approved, string? reason)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId))
            {
                return Unauthorized();
            }

            if (!await _userServices.ReviewOwnerRegistration(userId, approved, reason, adminId))
            {
                return Conflict(new { message = "Hồ sơ không tồn tại hoặc không còn ở trạng thái chờ duyệt." });
            }

            return Ok(new
            {
                message = approved
                    ? "Đã duyệt hồ sơ. Email thông báo đã được gửi vào hàng đợi."
                    : "Đã từ chối hồ sơ. Email thông báo đã được gửi vào hàng đợi."
            });
        }
        
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var username = request?.Username;
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new
                {
                    message = "Vui lòng nhập username."
                });
            }

            await _userServices.RequestPasswordReset(username);

            return Ok(new
            {
                message = "Nếu tài khoản tồn tại, mã xác nhận đã được gửi đến email."
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(new { message = "Username, mã xác nhận và mật khẩu mới là bắt buộc." });
            }

            await _userServices.ConfirmPasswordReset(request.Username, request.Code, request.NewPassword);
            return Ok(new { message = "Đặt lại mật khẩu thành công." });
        }
    }
}

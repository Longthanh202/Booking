using Booking.Core.Model.UserRoles;
using Booking.Core.Model.Users;
using Booking.Data.Repository.Auths;
using Booking.Data.Repository.Roles;
using Booking.Service.Dtos.Auths;
using Booking.Service.Services.Tokens;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.Auths
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService; // Service sinh JWT Token của bạn
        private readonly IRoleRepository _roleRepository;

        public AuthService(IAuthRepository authRepository, IConfiguration config, ITokenService tokenService,
            IRoleRepository roleRepository)
        {
            _authRepository = authRepository;
            _config = config;
            _tokenService = tokenService;
            _roleRepository = roleRepository;
        }

        public async Task<AuthResponseDto> LoginWithGoogleAsync(string googleIdToken, Guid defaultRoleId)
        {
            // 1. Xác thực Token từ Google
            GoogleJsonWebSignature.Payload googleUser;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _config["Authentication:Google:ClientId"] }
                };
                googleUser = await GoogleJsonWebSignature.ValidateAsync(googleIdToken, settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Google Token không hợp lệ hoặc đã hết hạn!", ex);
            }

            string googleSubId = googleUser.Subject;
            string email = googleUser.Email;
            string fullName = googleUser.Name;
            string avatar = googleUser.Picture; // Lấy URL Avatar từ Google

            // 2. Tìm liên kết Social đã tồn tại
            var externalLogin = await _authRepository.GetExternalLoginAsync("Google", googleSubId);
            UserLogin userLogin;
            UserProfile? userProfile;

            if (externalLogin != null)
            {
                // 🌟 TRƯỜNG HỢP 1: Đã từng đăng nhập bằng Google trước đó
                userLogin = externalLogin.UserLogin!;
                userProfile = userLogin.UserProfile;
            }
            else
            {
                // 3. Nếu chưa liên kết -> Tìm theo Email
                var existingProfile = await _authRepository.GetProfileByEmailAsync(email);

                if (existingProfile != null)
                {
                    // 🌟 TRƯỜNG HỢP 2: Đã có tài khoản Email -> Liên kết thêm Google
                    userLogin = existingProfile.UserLogin!;
                    userProfile = existingProfile;          

                    var newExternalLogin = new ExternalLogin
                    {
                        Id = Guid.NewGuid(),
                        UserLoginId = userLogin.Id,
                        Provider = "Google",
                        ProviderKey = googleSubId,
                        CreatedAt = DateTime.Now
                    };

                    await _authRepository.AddExternalLoginAsync(newExternalLogin);
                    await _authRepository.SaveChangesAsync();
                }
                else
                {
                    // 🌟 TRƯỜNG HỢP 3: Khách hàng mới -> Tạo toàn bộ thông tin
                    var newLoginId = Guid.NewGuid();

                    userLogin = new UserLogin
                    {
                        Id = newLoginId,
                        Username = email,
                        Password = null
                    };

                    userProfile = new UserProfile
                    {
                        Id = Guid.NewGuid(),
                        UserLoginId = newLoginId,
                        FullName = fullName,
                        Email = email,                   
                        CreateAt = DateTime.Now,
                        IsDel = 0
                    };

                    var userRole = new UserRole
                    {
                        Id = Guid.NewGuid(),
                        UserId = newLoginId,
                        RoleId = defaultRoleId
                    };

                    var newExternalLogin = new ExternalLogin
                    {
                        Id = Guid.NewGuid(),
                        UserLoginId = newLoginId,
                        Provider = "Google",
                        ProviderKey = googleSubId,
                        Avatar = avatar,
                        CreatedAt = DateTime.Now
                    };                 
                    await _authRepository.CreateUserWithSocialAsync(userLogin, userProfile, userRole, newExternalLogin);
                    await _authRepository.SaveChangesAsync();
                }
            }
            var role = await _roleRepository.GetById(defaultRoleId);
            userProfile.RoleName = role.RoleName;
            userProfile.RoleId = role.Id;

            // 4. Sinh JWT Token cho hệ thống
            string token = _tokenService.GenerateAccessToken(userProfile);

            return new AuthResponseDto
            {
                Token = token,
                FullName = userProfile?.FullName ?? string.Empty,
                Email = userProfile?.Email ?? string.Empty,               
            };
        }
    }
}

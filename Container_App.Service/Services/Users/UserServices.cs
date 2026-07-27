using Container_App.Common.Shared;
using Container_App.Core.Model.RefreshTokens;
using Container_App.Core.Model.Users;
using Container_App.Data;
using Container_App.Data.Connection;
using Container_App.Data.Repository.Permissions;
using Container_App.Data.Repository.RabbitMQ;
using Container_App.Data.Repository.Redis;
using Container_App.Data.Repository.RefreshTokens;
using Container_App.Data.Repository.Users;
using Container_App.Service.Dtos.Email;
using Container_App.Service.Dtos.Login;
using Container_App.Service.Dtos.UserProfile;
using Container_App.Service.Services.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Container_App.Service.Services.Users
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;       
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRedisService _redisService;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;
        public UserServices(IUserRepository userRepository, ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository, IHttpContextAccessor httpContextAccessor,
            IRedisService redisService, IPermissionRepository permissionRepository, IUnitOfWork unitOfWork,
            IRabbitMQPublisher rabbitMQPublisher)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _httpContextAccessor = httpContextAccessor;
            _redisService = redisService;
            _permissionRepository = permissionRepository;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _rabbitMQPublisher = rabbitMQPublisher;
        }
        public async Task<UserProfileResponse> GetById(Guid id, Guid roleId)
        {
            var cacheKey = $"Permission_Role_{roleId}";
            List<string>? permissionKeys;

            var cacheValue = await _redisService.Get(cacheKey);

            if (!string.IsNullOrEmpty(cacheValue))
            {
                permissionKeys = JsonSerializer.Deserialize<List<string>>(cacheValue);
            }
            else
            {
                var permissions = await _permissionRepository.GetListPermissionByUser(id);

                permissionKeys = permissions
                    .Select(x => $"{x.ResourceName.ToLower()}_{x.Action.ToLower()}")
                    .ToList();

                await _redisService.SetObject(
                    cacheKey,   
                    permissionKeys,
                    TimeSpan.FromMinutes(20));
            }

            var user = await _userRepository.GetById(id);

            return new UserProfileResponse
            {
                FullName = user.FullName,
                Permissions = permissionKeys!
            };
        }   

        public bool IsAuthenticated()
        {
            return _userRepository.IsAuthenticated();
        }

        public async Task<LoginReponse> Login(LoginResquest input)
        {
            if (string.IsNullOrEmpty(input.username) || string.IsNullOrEmpty(input.password))
            {
                return new LoginReponse
                {
                    status = false,
                    token = null,
                    message = "Username hoặc Password không được để trống"
                };
            }
            string passwordHash = Hash.HashPassword(input.password);
            var user = await _userRepository.Login(input.username, passwordHash);
            if (user == null)
            {
                return new LoginReponse
                {
                    status = false,
                    token = null,
                    message = "Username hoặc Password không đúng"
                };
            }

            var userProfile = new UserProfile
            {
                Id = user.Id,
                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,
                Address = user.Address,
                RoleId = user.RoleId,
                RoleName = user.RoleName
            };

            var token = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _refreshTokenRepository.InsertRefreshToken(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = 30,
                CreatedDate = DateTime.Now,
                Status = 1,
                FullName = user.FullName,
                RoleId = user.RoleId,
                RoleName = user.RoleName,

            });

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(
                "refreshToken",
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(30)
                });

            return new LoginReponse
            {
                status = true,
                token = token,
                message = "Đăng nhập thành công"
            };
        }

        public async Task<UserProfile> Register(UserRequset user)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var userLoginId = Guid.NewGuid();
                await _userRepository.InsertUserLogin(new UserLogin
                {
                    Id = userLoginId,
                    Username = user.Username,
                    Password = Hash.HashPassword(user.Password),
                });
                var profile = new UserProfile
                {
                    Id = Guid.NewGuid(),                   
                    FullName = user.FullName,
                    Phone = user.Phone,
                    Email = user.Email,
                    Address = user.Address,
                    IsDel = 0,
                    CreateAt = DateTime.Now,
                    CreateBy = user.CreateBy,
                    RoleId = user.RoleId,
                    UserLoginId = userLoginId,
                };

                await _userRepository.Insert(profile);
                await _unitOfWork.CommitAsync();
                await _rabbitMQPublisher.PublishAsync(
                "register_email_queue",
                new SendEmailEvent
                {
                    ToEmail = user.Email,
                    Subject =
                        "Đăng ký tài khoản thành công",

                    Body =
                        $"Xin chào {user.FullName}"
                });
                return profile;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine($"Error occurred while registering user: {ex.Message}");
                throw;
            }
        }
    }
}

using Container_App.Core.Model.RefreshTokens;
using Container_App.Core.Model.TienIchs;
using Container_App.Core.Model.Users;
using Container_App.Data.Connection;
using Container_App.Data.Repository.RefreshTokens;
using Container_App.Service.Dtos.UserProfile;
using Container_App.Service.Services.Tokens;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.RefreshTokens
{
    public class RefreshTokenService: IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, 
            IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }

        public async Task<RefreshToken> CheckStatusefreshToken(string token)
        {
            try
            {
                return await _refreshTokenRepository.CheckStatusefreshToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when CheckStatusefreshToken: " + ex.Message);
                return null;
           
            }
        }

        public async Task<RefreshToken> InsertRefreshToken(RefreshToken refreshToken)
        {
            try
            {
                return await _refreshTokenRepository.InsertRefreshToken(refreshToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when InsertRefreshToken: " + ex.Message);
                return null;
            }
        }

        public async Task<string?> RefreshToken()
        {
            var refreshToken = _httpContextAccessor.HttpContext?
                .Request
                .Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var existingToken = await _refreshTokenRepository.CheckStatusefreshToken(refreshToken);

            if (existingToken == null)
                return null;

            var accessToken = _tokenService.GenerateAccessToken(new UserProfile
            {
                Id = existingToken.UserId.Value,
                FullName = existingToken.FullName,
                RoleId = existingToken.RoleId.Value,
                RoleName = existingToken.RoleName
            });

            return accessToken;
        }
    }
}

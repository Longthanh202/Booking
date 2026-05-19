using Container_App.Core.Interface.RefreshTokens;
using Container_App.Core.Model.RefreshTokens;
using Container_App.Core.Model.TienIchs;
using Container_App.Core.Model.Users;
using Container_App.Data.Connection;
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
        private readonly IStoredProcedureExecutor _executor;
        public RefreshTokenService(IStoredProcedureExecutor executor)
        {
            _executor = executor;
        }

        public async Task<RefreshToken> CheckStatusefreshToken(string token)
        {
            try
            {
                var arr = new[]
                {
                    new SqlParameter("@Token", token)
                };
                return await _executor.QuerySingleAsync<RefreshToken>("sp_CheckStatusRefreshToken", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error when CheckStatusefreshToken");

                throw;
            }
        }

        public async Task<int> InsertRefreshToken(RefreshToken refreshToken)
        {
            try
            {
                var arr = new[]
                {
                    new SqlParameter("@UserId", refreshToken.UserId),
                    new SqlParameter("@Token", refreshToken.Token),
                    new SqlParameter("@ExpiryDate", refreshToken.ExpireDate),
                    new SqlParameter("@FullName", refreshToken.FullName),
                    new SqlParameter("@RoleId", refreshToken.RoleId),
                    new SqlParameter("@RoleName", refreshToken.RoleName)
                };
                return await _executor.ExecuteAsync("sp_InsertRefreshToken", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when inserting refresh token.");

                return -1;
            }
        }
    }
}

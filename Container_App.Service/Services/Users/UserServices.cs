using Container_App.Common.Shared;
using Container_App.Core.Interface.Users;
using Container_App.Core.Model.Users;
using Container_App.Data.Connection;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.Users
{
    public class UserServices : IUserServices
    {
        private readonly IStoredProcedureExecutor _executor;
        private readonly IHttpContextAccessor _httpContextAccessor; 
        public UserServices(IStoredProcedureExecutor executor, IHttpContextAccessor httpContextAccessor)
        {
            _executor = executor;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<UserProfile> GetById(Guid id)
        {
            try
            {
                var arr = new[]
                {
                    new SqlParameter("@Id", id)
                };
                return await _executor.QuerySingleAsync<UserProfile>("sp_GetProfile", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error when GetById");

                throw;
            }
        }

        public async Task<int> Insert(UserProfile user)
        {
            try
            {
                string hash = Hash.HashPassword(user.Password);
                var arr = new[]
                {
                    new SqlParameter("@Username", user.UserName),
                    new SqlParameter("@Password", hash),
                    new SqlParameter("@FullName", user.FullName),
                    new SqlParameter("@Phone", user.Phone),
                    new SqlParameter("@Email", user.Email),
                    new SqlParameter("@Address", user.Address),
                    new SqlParameter("@CreateBy", (object?)user.CreateBy ?? DBNull.Value),
                    new SqlParameter("@RoleId", user.RoleId)
                };
                return await _executor.ExecuteAsync("sp_CreateUser", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when creating user. Username={Username}, Email={Email}", user.UserName, user.Email);

                return -1;
            }
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public async Task<UserProfile> Login(string userName, string passWord)
        {
            try
            {
                string hash = Hash.HashPassword(passWord);
                var arr = new[]
                {
                    new SqlParameter("@Username", userName),
                    new SqlParameter("@Password", hash),                  
                };
                return await _executor.QuerySingleAsync<UserProfile>("sp_Login", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,"Error when login");
                
                throw;
            }
        }
    }
}

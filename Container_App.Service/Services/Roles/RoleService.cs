using Container_App.Common.Shared;
using Container_App.Core.Model.Users;
using Container_App.Data.Connection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IStoredProcedureExecutor _executor;
       public RoleService(IStoredProcedureExecutor executor)
        {
            _executor = executor;
        }
        public async Task<Role> CheckRoleAdmin(Guid userId)
        {
            try
            {              
                var arr = new[]
                {
                    new SqlParameter("@UserId", userId),                  
                };
                return await _executor.QuerySingleAsync<Role>("sp_CheckRoleAdmin", arr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error when check role admin");
                throw;
            }
        }
    }
}

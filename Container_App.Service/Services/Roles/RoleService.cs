using Container_App.Common.Shared;
using Container_App.Core.Model.Roles;
using Container_App.Core.Model.Users;
using Container_App.Data.Connection;
using Container_App.Data.Repository.Roles;
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
        private readonly IRoleRepository _roleRepository;
       public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<Role> CheckRoleAdmin(Guid userId)
        {
            try
            {              
                return await _roleRepository.CheckRoleAdmin(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error when check role admin");
                throw;
            }
        }
    }
}

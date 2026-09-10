using Booking.Common.Shared;
using Booking.Core.Model.Roles;
using Booking.Core.Model.Users;
using Booking.Data.Connection;
using Booking.Data.Repository.Roles;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.Roles
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
            return await _roleRepository.CheckRoleAdmin(userId);
        }
    }
}

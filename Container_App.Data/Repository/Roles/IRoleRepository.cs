using Container_App.Core.Model.Roles;
using Container_App.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Roles
{
    public interface IRoleRepository
    {
        Task<Role> CheckRoleAdmin(Guid userId);
    }
}

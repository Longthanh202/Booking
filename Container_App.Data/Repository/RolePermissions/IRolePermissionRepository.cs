using Container_App.Core.Model.RolePermissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.RolePermissions
{
    public interface IRolePermissionRepository
    {
        Task Insert(Guid roleId, List<RolePermission> lst);
    }
}

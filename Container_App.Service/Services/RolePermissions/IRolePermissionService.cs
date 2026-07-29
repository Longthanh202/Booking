using Container_App.Core.Model.RolePermissions;
using Container_App.Service.Dtos.RolePermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.RolePermissions
{
    public interface IRolePermissionService
    {
        Task Insert(RolePermissionRequset input);
    }
}

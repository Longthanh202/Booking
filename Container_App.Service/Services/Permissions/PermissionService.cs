using Container_App.Common.Shared;
using Container_App.Core.Model.Permissions;
using Container_App.Core.Model.Users;
using Container_App.Data.Connection;
using Container_App.Data.Repository.Permissions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.Permissions
{
    public class PermissionService: IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<List<PermissionInfo>> GetListPermissionByUser(Guid userId)
        {
            return await _permissionRepository.GetListPermissionByUser(userId);
        }

        public bool HasPermission(List<PermissionInfo> userPermissions, string resource, string action)
        {
            return userPermissions.Any(p =>
                p.ResourceName.Equals(resource, StringComparison.OrdinalIgnoreCase)
                && p.Action.Equals(action, StringComparison.OrdinalIgnoreCase)
            );
        }
    }
}

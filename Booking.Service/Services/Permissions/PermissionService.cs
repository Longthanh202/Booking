using Booking.Common.Shared;
using Booking.Core.Model.Permissions;
using Booking.Core.Model.Users;
using Booking.Data.Connection;
using Booking.Data.Repository.Permissions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.Permissions
{
    public class PermissionService: IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<List<PermissionInfo>> GetPermissionsByUserId(Guid userId)
        {
            return await _permissionRepository.GetPermissionsByUserId(userId);
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

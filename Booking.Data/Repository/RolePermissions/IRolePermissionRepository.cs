using Booking.Core.Model.RolePermissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.RolePermissions
{
    public interface IRolePermissionRepository
    {
        Task Insert(Guid roleId, List<RolePermission> lst);
    }
}

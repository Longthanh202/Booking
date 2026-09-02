using Booking.Core.Model.RolePermissions;
using Booking.Service.Dtos.RolePermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.RolePermissions
{
    public interface IRolePermissionService
    {
        Task Insert(RolePermissionRequset input);
    }
}

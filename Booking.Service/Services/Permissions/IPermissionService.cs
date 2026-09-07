using Booking.Core.Model.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Permissions
{
    public interface IPermissionService
    {
        Task<List<PermissionInfo>> GetListPermissionByUser(Guid userId);
    }
}

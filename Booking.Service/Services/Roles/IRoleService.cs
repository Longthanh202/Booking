using Booking.Core.Model.Roles;
using Booking.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.Roles
{
    public interface IRoleService
    {
        Task<Role> CheckRoleAdmin(Guid userId);
    }
}

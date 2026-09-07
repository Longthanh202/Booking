using Booking.Core.Model.Roles;
using Booking.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Roles
{
    public interface IRoleRepository
    {
        Task<Role> CheckRoleAdmin(Guid userId);
        Task<Role?> GetById(Guid id);
        Task<Role?> GetRoleCustomer();
    }
}

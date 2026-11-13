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
        Task<VaiTro> CheckRoleAdmin(Guid userId);
        Task<VaiTro?> GetById(Guid id);
        Task<VaiTro?> GetRoleCustomer();
        Task<VaiTro?> GetRoleOwner();
    }
}

using Booking.Core.Model.Roles;
using Booking.Core.Model.Users;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Roles
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;
        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Role?> CheckRoleAdmin(Guid userId)
        {
            return await (
                from ur in _context.UserRoles
                join r in _context.Roles
                    on ur.RoleId equals r.Id
                where ur.UserId == userId
                select r
            ).FirstOrDefaultAsync();
        }

        public async Task<Role?> GetById(Guid id)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Role?> GetRoleCustomer()
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == "Customer");
        }
    }
}

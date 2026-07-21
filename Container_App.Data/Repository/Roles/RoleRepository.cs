using Container_App.Core.Model.Roles;
using Container_App.Core.Model.Users;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Roles
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
    }
}

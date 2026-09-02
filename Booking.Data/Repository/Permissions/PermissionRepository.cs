using Booking.Core.Model.Permissions;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Permissions
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;
        public PermissionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<PermissionInfo>> GetListPermissionByUser(Guid userId)
        {
            return await (
                from ur in _context.UserRoles
                join r in _context.Roles
                    on ur.RoleId equals r.Id
                join rp in _context.RolePermissions
                    on r.Id equals rp.RoleId
                join res in _context.Resources
                    on rp.ResourceId equals res.Id
                join p in _context.Permissions
                    on rp.PermissionId equals p.Id
                where ur.UserId == userId
                orderby res.ResourceName, p.Action
                select new PermissionInfo
                {
                    ResourceName = res.ResourceName,
                    Action = p.Action
                })
                .Distinct()
                .ToListAsync();
        }
    }
}

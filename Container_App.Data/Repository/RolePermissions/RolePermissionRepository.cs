using Container_App.Core.Model.RolePermissions;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.RolePermissions
{
    public class RolePermissionRepository: IRolePermissionRepository
    {
        private readonly AppDbContext _context;
        public RolePermissionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task Insert(Guid roleId, List<RolePermission> lst)
        {
            var oldPermissions = await _context.RolePermissions
                .Where(x => x.RoleId == roleId).ToListAsync();

            _context.RolePermissions.RemoveRange(oldPermissions);
            foreach (var item in lst)
            {
                item.Id = Guid.NewGuid();
                item.RoleId = roleId;
            }
            await _context.RolePermissions.AddRangeAsync(lst);
            
        }
    }
}

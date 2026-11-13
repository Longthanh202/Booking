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
        public async Task<VaiTro?> CheckRoleAdmin(Guid userId)
        {
            return await (
                from ur in _context.NguoiDung_VaiTros
                join r in _context.VaiTros
                    on ur.RoleId equals r.Id
                where ur.UserId == userId && r.RoleName == "Admin"
                select r
            ).FirstOrDefaultAsync();
        }

        public async Task<VaiTro?> GetById(Guid id)
        {
            return await _context.VaiTros.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<VaiTro?> GetRoleCustomer()
        {
            return await _context.VaiTros.FirstOrDefaultAsync(x => x.RoleName == "Customer");
        }

        public Task<VaiTro?> GetRoleOwner() =>
            _context.VaiTros.FirstOrDefaultAsync(x => x.RoleName == "Owner");
    }
}

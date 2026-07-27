using Container_App.Core.Model.Users;
using Container_App.Data.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<UserProfile?> GetById(Guid id)
        {
            var user = await _context.UserProfiles
                .Where(u => u.UserLoginId == id)
                .Select(u => new UserProfile
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Phone = u.Phone,
                    Email = u.Email,
                    Address = u.Address
                })
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<UserProfile> Insert(UserProfile user)
        {
            await _context.UserProfiles.AddAsync(user);        
            return user;
        }

        public async Task<UserLogin> InsertUserLogin(UserLogin userLogin)
        {
            await _context.UserLogins.AddAsync(userLogin);
            return userLogin;
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public async Task<UserProfile?> Login(string userName, string passWord)
        {
            try
            {
                return await (
                 from ul in _context.UserLogins
                 join up in _context.UserProfiles on ul.Id equals up.UserLoginId
                 join ur in _context.UserRoles on ul.Id equals ur.UserId
                 join r in _context.Roles on ur.RoleId equals r.Id
                 where ul.Username == userName && ul.Password == passWord
                 && up.IsDel == 0
             select new UserProfile
             {
                 Id = ul.Id,
                 FullName = up.FullName,
                 Phone = up.Phone,
                 Email = up.Email,
                 Address = up.Address,
                 RoleId = r.Id,
                 RoleName = r.RoleName

             }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during login: {ex.Message}");
                return null;
            }
        }
    }
}

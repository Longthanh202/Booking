using Booking.Core.Model.Users;
using Booking.Data.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Common.Shared;

namespace Booking.Data.Repository.Users
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

        public async Task<UserProfile> QuenMatKhau(string username)
        {
            try
            {
                return await (
                    from ul in _context.UserLogins
                    join up in _context.UserProfiles on ul.Id equals up.UserLoginId
                    where ul.Username == username && up.IsDel == 0
                    select new UserProfile
                    {
                        Id = ul.Id,
                        FullName = up.FullName,
                        Phone = up.Phone,
                        Email = up.Email,
                        Address = up.Address
                    }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error QuenMatKhau: {ex.Message}");
                return null;
            }
        }

        public async Task<UserLogin> UpdatePassword(
            string username,
            string password)
        {
            var user = await _context.UserLogins
                .FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
            {
                throw new Exception("User không tồn tại");
            }

            _context.UserLogins.Update(user);

            await _context.SaveChangesAsync();

            return user;
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
                FileLogger.Log(ex);
                return null;
            }
        }
    }
}

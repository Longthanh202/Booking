using Container_App.Core.Model.UserRoles;
using Container_App.Core.Model.Users;
using Container_App.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Auths
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ExternalLogin?> GetExternalLoginAsync(string provider, string providerKey)
        {
            return await _context.Set<ExternalLogin>()
                .Include(el => el.UserLogin)
                    .ThenInclude(ul => ul.UserProfile)
                .FirstOrDefaultAsync(el => el.Provider == provider && el.ProviderKey == providerKey);
        }

        public async Task<UserProfile?> GetProfileByEmailAsync(string email)
        {
            return await _context.UserProfiles
                .Include(p => p.UserLogin)
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task CreateUserWithSocialAsync(UserLogin userLogin, UserProfile userProfile, UserRole userRole, ExternalLogin externalLogin)
        {
            await _context.UserLogins.AddAsync(userLogin);
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.Set<UserRole>().AddAsync(userRole);
            await _context.Set<ExternalLogin>().AddAsync(externalLogin);
        }

        public async Task AddExternalLoginAsync(ExternalLogin externalLogin)
        {
            await _context.Set<ExternalLogin>().AddAsync(externalLogin);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

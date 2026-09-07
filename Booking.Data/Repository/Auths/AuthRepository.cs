using Booking.Common.Shared;
using Booking.Core.Model.UserRoles;
using Booking.Core.Model.Users;
using Booking.Data.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data.Repository.Auths
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
            try
            {
                return await _context.Set<ExternalLogin>()
                .Include(el => el.UserLogin)
                    .ThenInclude(ul => ul.UserProfile)
                .FirstOrDefaultAsync(el => el.Provider == provider && el.ProviderKey == providerKey);
            }
            catch(Exception ex)
            {
                FileLogger.Log(ex);
                return null;
            }
        }

        public async Task<UserProfile?> GetProfileByEmailAsync(string email)
        {
            try
            {
                return await _context.UserProfiles
                .Include(p => p.UserLogin)
                .FirstOrDefaultAsync(p => p.Email == email);
            }
            catch(Exception ex)
            {
                FileLogger.Log(ex);
                return null;
            }
        }

        public async Task CreateUserWithSocialAsync(UserLogin userLogin, UserProfile userProfile, UserRole userRole, ExternalLogin externalLogin)
        {
            try
            {
                await _context.UserLogins.AddAsync(userLogin);
                await _context.UserProfiles.AddAsync(userProfile);
                await _context.Set<UserRole>().AddAsync(userRole);
                await _context.Set<ExternalLogin>().AddAsync(externalLogin);
            }
            catch(Exception ex)
            {
                FileLogger.Log(ex);
            }
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

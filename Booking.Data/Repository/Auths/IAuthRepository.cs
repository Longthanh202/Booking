using Booking.Core.Model.UserRoles;
using Booking.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Auths
{
    public interface IAuthRepository
    {
        Task<ExternalLogin?> GetExternalLoginAsync(string provider, string providerKey);
        Task<UserProfile?> GetProfileByEmailAsync(string email);
        Task CreateUserWithSocialAsync(UserLogin userLogin, UserProfile userProfile, UserRole userRole, ExternalLogin externalLogin);
        Task AddExternalLoginAsync(ExternalLogin externalLogin);
        Task SaveChangesAsync();
    }
}

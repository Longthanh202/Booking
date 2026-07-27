using Container_App.Core.Model.UserRoles;
using Container_App.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Auths
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

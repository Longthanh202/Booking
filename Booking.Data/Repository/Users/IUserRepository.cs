using Booking.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Users
{
    public interface IUserRepository
    {
        Task<UserProfile> AddUserProfile(UserProfile user);
        Task<UserLogin> AddUserLogin(UserLogin userLogin);
        Task<UserProfile?> Login(string userName, string passWord);
        Task<UserProfile?> GetUserProfileById(Guid id);
        bool IsAuthenticated();
        Task<UserProfile?> FindUserByUsernameForPasswordReset(string username);
        Task<UserLogin> UpdatePassword(string username, string passage);
    }
}

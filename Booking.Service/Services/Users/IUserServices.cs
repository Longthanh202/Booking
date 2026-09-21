using Booking.Core.Model.Users;
using Booking.Service.Dtos.Login;
using Booking.Service.Dtos.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Users
{
    public interface IUserServices
    {     
        Task<LoginReponse> Login(LoginResquest input);
        Task<UserProfileResponse> GetUserProfileById(Guid id, Guid roleId);
        bool IsAuthenticated();
        Task<UserProfile> Register(UserRequset user);
        Task RequestPasswordReset(string username);
        Task ConfirmPasswordReset(string username, string code);
    }
}

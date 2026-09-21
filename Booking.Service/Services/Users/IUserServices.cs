using Booking.Core.Model.Users;
using Booking.Service.Dtos.Authentication;
using Booking.Service.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Users
{
    public interface IUserServices
    {     
        Task<LoginResponse> Login(LoginRequest input);
        Task<UserProfileResponse> GetUserProfileById(Guid id, Guid roleId);
        bool IsAuthenticated();
        Task<UserProfile> Register(RegisterUserRequest user);
        Task RequestPasswordReset(string username);
        Task ConfirmPasswordReset(string username, string code);
    }
}

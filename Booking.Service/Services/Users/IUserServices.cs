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
        Task<UserProfileResponse> GetById(Guid id, Guid roleId);
        bool IsAuthenticated();
        Task<UserProfile> Register(UserRequset user);
        Task QuenMatKhau(string username);
        Task ComfirmQuenMatKhau(string username, string code);
    }
}

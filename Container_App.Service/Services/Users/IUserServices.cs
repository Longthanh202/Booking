using Container_App.Core.Model.Users;
using Container_App.Service.Dtos.Login;
using Container_App.Service.Dtos.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Users
{
    public interface IUserServices
    {
        Task<UserProfile> Insert(UserProfile user);
        Task<LoginReponse> Login(LoginResquest input);
        Task<UserProfileResponse> GetById(Guid id, Guid roleId);
        bool IsAuthenticated();
        Task<UserProfile> Register(UserRequset user);
    }
}

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
        Task<UserProfile> Insert(UserProfile user);
        Task<UserLogin> InsertUserLogin(UserLogin userLogin);
        Task<UserProfile> Login(string userName, string passWord);
        Task<UserProfile> GetById(Guid id);
        bool IsAuthenticated();
        Task<UserProfile> QuenMatKhau(string username);
        Task<UserLogin> UpdatePassword(string username, string passage);
    }
}

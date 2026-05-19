using Container_App.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Interface.Users
{
    public interface IUserServices
    {
        Task<int> Insert(UserProfile user);
        Task<UserProfile> Login(string userName, string passWord);
        Task<UserProfile> GetById(Guid id); 
    }
}

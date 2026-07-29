using Container_App.Core.Model.Users;
using Container_App.Service.Dtos.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.Tokens
{
    public interface ITokenService
    {
        string GenerateAccessToken(UserProfile user);

        string GenerateRefreshToken();
    }
}

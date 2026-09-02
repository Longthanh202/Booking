using Booking.Core.Model.Users;
using Booking.Service.Dtos.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.Tokens
{
    public interface ITokenService
    {
        string GenerateAccessToken(UserProfile user);

        string GenerateRefreshToken();
    }
}

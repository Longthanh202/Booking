using Booking.Service.Dtos.Auths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.Auths
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginWithGoogleAsync(string googleIdToken, Guid defaultRoleId);
    }
}

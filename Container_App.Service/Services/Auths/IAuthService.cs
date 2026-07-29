using Container_App.Service.Dtos.Auths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.Auths
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginWithGoogleAsync(string googleIdToken, Guid defaultRoleId);
    }
}

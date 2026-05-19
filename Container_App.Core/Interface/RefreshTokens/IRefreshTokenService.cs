using Container_App.Core.Model.RefreshTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Interface.RefreshTokens
{
    public interface IRefreshTokenService
    {
        Task<int> InsertRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> CheckStatusefreshToken(string token);
    }
}

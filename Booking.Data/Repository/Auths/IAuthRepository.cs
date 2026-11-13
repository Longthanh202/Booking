using Booking.Core.Model.UserRoles;
using Booking.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Auths
{
    public interface IAuthRepository
    {
        Task<DangNhapNgoai?> GetExternalLoginAsync(string provider, string providerKey);
        Task<NguoiDung?> GetProfileByEmailAsync(string email);
        Task CreateUserWithSocialAsync(TaiKhoan userLogin, NguoiDung userProfile, NguoiDung_VaiTro userRole, DangNhapNgoai externalLogin);
        Task AddExternalLoginAsync(DangNhapNgoai externalLogin);
        Task SaveChangesAsync();
    }
}

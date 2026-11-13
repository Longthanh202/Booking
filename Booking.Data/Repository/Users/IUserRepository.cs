using Booking.Core.Model.Users;
using Booking.Core.Model.TaiKhoanNganHangs;
using Booking.Core.Model.UserRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Users
{
    public interface IUserRepository
    {
        Task<NguoiDung> AddUserProfile(NguoiDung user);
        Task<TaiKhoan> AddUserLogin(TaiKhoan userLogin);
        Task<NguoiDung?> Login(string userName, string passWord);
        Task<NguoiDung?> GetUserProfileById(Guid id);
        bool IsAuthenticated();
        Task<NguoiDung?> FindUserByUsernameForPasswordReset(string username);
        Task<TaiKhoan> UpdatePassword(string username, string passage);
        Task<(List<NguoiDung> Users, int TotalCount)> GetAdminUsers(string? keyword, int? status, int page, int pageSize);
        Task<bool> SetAccountStatus(Guid userId, int status);
        Task<bool> IsUsernameOrEmailTaken(string username, string email);
        Task AddUserRole(NguoiDung_VaiTro userRole);
        Task AddBankAccount(TaiKhoanNganHang bankAccount);
        Task<(List<NguoiDung> Users, int TotalCount)> GetOwnerRegistrations(
            int? approvalStatus, string? keyword, int page, int pageSize);
        Task<NguoiDung?> ReviewOwnerRegistration(
            Guid userId, int approvalStatus, string? reviewNote, Guid adminId);
        Task<TaiKhoan> GetUserLoginByUserName(string userName);
    }
}

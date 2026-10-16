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
        Task<UserProfile> AddUserProfile(UserProfile user);
        Task<UserLogin> AddUserLogin(UserLogin userLogin);
        Task<UserProfile?> Login(string userName, string passWord);
        Task<UserProfile?> GetUserProfileById(Guid id);
        bool IsAuthenticated();
        Task<UserProfile?> FindUserByUsernameForPasswordReset(string username);
        Task<UserLogin> UpdatePassword(string username, string passage);
        Task<(List<UserProfile> Users, int TotalCount)> GetAdminUsers(string? keyword, int? status, int page, int pageSize);
        Task<bool> SetAccountStatus(Guid userId, int status);
        Task<bool> IsUsernameOrEmailTaken(string username, string email);
        Task AddUserRole(UserRole userRole);
        Task AddBankAccount(TaiKhoanNganHang bankAccount);
        Task<(List<UserProfile> Users, int TotalCount)> GetOwnerRegistrations(
            int? approvalStatus, string? keyword, int page, int pageSize);
        Task<UserProfile?> ReviewOwnerRegistration(
            Guid userId, int approvalStatus, string? reviewNote, Guid adminId);
    }
}

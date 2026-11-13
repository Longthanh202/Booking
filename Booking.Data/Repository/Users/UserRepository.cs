using Booking.Core.Model.Users;
using Booking.Core.Model.TaiKhoanNganHangs;
using Booking.Core.Model.UserRoles;
using Booking.Data.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Common.Shared;

namespace Booking.Data.Repository.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<NguoiDung?> GetUserProfileById(Guid id)
        {
            var user = await _context.NguoiDungs
                .Where(u => u.UserLoginId == id)
                .Select(u => new NguoiDung
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Phone = u.Phone,
                    Email = u.Email,
                    Address = u.Address
                })
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<TaiKhoan> GetUserLoginByUserName(string userName)
        {
            var user = await _context.TaiKhoans
                .Where(o => o.Username == userName)
                .Select(o => new TaiKhoan
                {
                    Id = o.Id,
                    Username = o.Username,
                    Password = o.Password
                })
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<NguoiDung> AddUserProfile(NguoiDung user)
        {
            await _context.NguoiDungs.AddAsync(user);
            return user;
        }

        public async Task<TaiKhoan> AddUserLogin(TaiKhoan userLogin)
        {
            await _context.TaiKhoans.AddAsync(userLogin);
            return userLogin;
        }

        public async Task<bool> IsUsernameOrEmailTaken(string username, string email)
        {
            var normalizedUsername = username.Trim();
            var normalizedEmail = email.Trim();
            return await _context.TaiKhoans.AnyAsync(
                       login => login.Username == normalizedUsername) ||
                   await _context.NguoiDungs.AnyAsync(
                       profile => profile.Email == normalizedEmail);
        }

        public async Task AddUserRole(NguoiDung_VaiTro userRole) =>
            await _context.NguoiDung_VaiTros.AddAsync(userRole);

        public async Task AddBankAccount(TaiKhoanNganHang bankAccount) =>
            await _context.TaiKhoanNganHangs.AddAsync(bankAccount);

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public async Task<NguoiDung> FindUserByUsernameForPasswordReset(string username)
        {
            return await (
                    from ul in _context.TaiKhoans
                    join up in _context.NguoiDungs on ul.Id equals up.UserLoginId
                    where ul.Username == username && up.IsDel == 0
                    select new NguoiDung
                    {
                        Id = ul.Id,
                        FullName = up.FullName,
                        Phone = up.Phone,
                        Email = up.Email,
                        Address = up.Address
                    }).FirstOrDefaultAsync();
        }

        public async Task<TaiKhoan> UpdatePassword(
            string username,
            string password)
        {
            var user = await _context.TaiKhoans
                .FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
            {
                throw new Exception("User không tồn tại");
            }

            user.Password = Booking.Common.Shared.Hash.HashPassword(password);
            _context.TaiKhoans.Update(user);

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<NguoiDung?> Login(string userName, string passWord)
        {
            var result =  await (
                 from ul in _context.TaiKhoans
                 join up in _context.NguoiDungs on ul.Id equals up.UserLoginId
                 join ur in _context.NguoiDung_VaiTros on ul.Id equals ur.UserId
                 join r in _context.VaiTros on ur.RoleId equals r.Id
                 where ul.Username == userName && ul.Password == passWord
                 && up.IsDel == 0
             select new NguoiDung
             {
                 Id = ul.Id,
                 FullName = up.FullName,
                 Phone = up.Phone,
                 Email = up.Email,
                 Address = up.Address,
                 RoleId = r.Id,
                 RoleName = r.RoleName

             }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<(List<NguoiDung> Users, int TotalCount)> GetAdminUsers(
            string? keyword,
            int? status,
            int page,
            int pageSize)
        {
            var query =
                from profile in _context.NguoiDungs.AsNoTracking()
                join login in _context.TaiKhoans on profile.UserLoginId equals login.Id
                join userRole in _context.NguoiDung_VaiTros on login.Id equals userRole.UserId into userRoleGroup
                from userRole in userRoleGroup.DefaultIfEmpty()
                join role in _context.VaiTros on userRole.RoleId equals role.Id into roleGroup
                from role in roleGroup.DefaultIfEmpty()
                select new NguoiDung
                {
                    Id = profile.Id,
                    UserLoginId = login.Id,
                    Username = login.Username,
                    FullName = profile.FullName,
                    Phone = profile.Phone,
                    Email = profile.Email,
                    Address = profile.Address,
                    IsDel = profile.IsDel,
                    CreateAt = profile.CreateAt,
                    RoleId = userRole == null ? Guid.Empty : userRole.RoleId,
                    RoleName = role == null ? null : role.RoleName
                };

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var search = keyword.Trim();
                query = query.Where(user =>
                    (user.Username != null && user.Username.Contains(search)) ||
                    (user.FullName != null && user.FullName.Contains(search)) ||
                    (user.Email != null && user.Email.Contains(search)));
            }

            if (status.HasValue)
            {
                query = query.Where(user => user.IsDel == status.Value);
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .OrderByDescending(user => user.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<bool> SetAccountStatus(Guid userId, int status)
        {
            var profile = await _context.NguoiDungs
                .FirstOrDefaultAsync(user => user.UserLoginId == userId);
            if (profile == null)
            {
                return false;
            }

            if (status == 0 && await (
                    from userRole in _context.NguoiDung_VaiTros
                    join role in _context.VaiTros on userRole.RoleId equals role.Id
                    where userRole.UserId == userId &&
                          role.RoleName == "Owner" &&
                          profile.OwnerApprovalStatus != (int)OwnerRegistrationStatus.Approved
                    select userRole.Id).AnyAsync())
            {
                return false;
            }

            profile.IsDel = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(List<NguoiDung> Users, int TotalCount)> GetOwnerRegistrations(
            int? approvalStatus, string? keyword, int page, int pageSize)
        {
            var query =
                from profile in _context.NguoiDungs.AsNoTracking()
                join login in _context.TaiKhoans on profile.UserLoginId equals login.Id
                join userRole in _context.NguoiDung_VaiTros on login.Id equals userRole.UserId
                join role in _context.VaiTros on userRole.RoleId equals role.Id
                where role.RoleName == "Owner"
                select new NguoiDung
                {
                    Id = profile.Id,
                    UserLoginId = login.Id,
                    Username = login.Username,
                    FullName = profile.FullName,
                    Phone = profile.Phone,
                    Email = profile.Email,
                    Address = profile.Address,
                    IsDel = profile.IsDel,
                    CreateAt = profile.CreateAt,
                    OwnerApprovalStatus = profile.OwnerApprovalStatus,
                    OwnerReviewNote = profile.OwnerReviewNote,
                    OwnerReviewedAt = profile.OwnerReviewedAt,
                    OwnerReviewedBy = profile.OwnerReviewedBy,
                    BankAccount = _context.TaiKhoanNganHangs
                        .Where(bank => bank.UserId == login.Id)
                        .OrderByDescending(bank => bank.IsDefault)
                        .ThenBy(bank => bank.CreatedDate)
                        .Select(bank => new TaiKhoanNganHang
                        {
                            Id = bank.Id,
                            UserId = bank.UserId,
                            TenNganHang = bank.TenNganHang,
                            SoTaiKhoan = bank.SoTaiKhoan,
                            ChuTaiKhoan = bank.ChuTaiKhoan,
                            ChiNhanh = bank.ChiNhanh,
                            IsDefault = bank.IsDefault
                        })
                        .FirstOrDefault()
                };

            if (approvalStatus.HasValue)
            {
                query = query.Where(user => user.OwnerApprovalStatus == approvalStatus.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var search = keyword.Trim();
                query = query.Where(user =>
                    (user.Username != null && user.Username.Contains(search)) ||
                    (user.FullName != null && user.FullName.Contains(search)) ||
                    (user.Email != null && user.Email.Contains(search)) ||
                    (user.Phone != null && user.Phone.Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .OrderByDescending(user => user.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<NguoiDung?> ReviewOwnerRegistration(
            Guid userId, int approvalStatus, string? reviewNote, Guid adminId)
        {
            var profile = await (
                from userProfile in _context.NguoiDungs
                join userRole in _context.NguoiDung_VaiTros on userProfile.UserLoginId equals userRole.UserId
                join role in _context.VaiTros on userRole.RoleId equals role.Id
                join login in _context.TaiKhoans on userProfile.UserLoginId equals login.Id
                where login.Id == userId &&
                      role.RoleName == "Owner" &&
                      userProfile.OwnerApprovalStatus == 0
                select new { userProfile, login.Username }).FirstOrDefaultAsync();

            if (profile == null)
            {
                return null;
            }

            profile.userProfile.OwnerApprovalStatus = approvalStatus;
            profile.userProfile.IsDel = approvalStatus == 1 ? 0 : 1;
            profile.userProfile.OwnerReviewNote = reviewNote;
            profile.userProfile.OwnerReviewedAt = DateTime.UtcNow;
            profile.userProfile.OwnerReviewedBy = adminId;
            await _context.SaveChangesAsync();

            profile.userProfile.UserLoginId = userId;
            profile.userProfile.Username = profile.Username;
            return profile.userProfile;
        }
    }
}

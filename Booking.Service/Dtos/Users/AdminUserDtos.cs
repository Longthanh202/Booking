using System.ComponentModel.DataAnnotations;

namespace Booking.Service.Dtos.Users
{
    public class UpdateAccountStatusRequest
    {
        [Range(0, 1)]
        public int IsDel { get; set; }
    }

    public class AdminUserItemResponse
    {
        public Guid UserId { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? RoleName { get; set; }
        public int? IsDel { get; set; }
        public DateTime? CreateAt { get; set; }
    }

    public class AdminUserPageResponse
    {
        public List<AdminUserItemResponse> Data { get; set; } = new();
        public int TotalRow { get; set; }
        public int TotalPage { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Booking.Service.Dtos.Users
{
    public class OwnerRegistrationRequest
    {
        [Required, StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Required, Phone, StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Required, StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string BankName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string BankAccountNumber { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string BankAccountHolder { get; set; } = string.Empty;

        [StringLength(200)]
        public string? BankBranch { get; set; }
    }

    public class OwnerRegistrationItemResponse
    {
        public Guid UserId { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? ApprovalStatus { get; set; }
        public string? ReviewNote { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public OwnerBankAccountResponse? BankAccount { get; set; }
    }

    public class OwnerBankAccountResponse
    {
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolder { get; set; } = string.Empty;
        public string? Branch { get; set; }
    }

    public class OwnerRegistrationPageResponse
    {
        public List<OwnerRegistrationItemResponse> Data { get; set; } = new();
        public int TotalRow { get; set; }
        public int TotalPage { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class RejectOwnerRegistrationRequest
    {
        [Required, StringLength(500, MinimumLength = 1)]
        public string Reason { get; set; } = string.Empty;
    }
}

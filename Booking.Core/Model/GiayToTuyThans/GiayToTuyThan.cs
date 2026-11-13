using Booking.Core.Model.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Core.Model.GiayToTuyThans
{
    public class GiayToTuyThan
    {
        [Key]
        public long Id { get; set; }

        public Guid UserProfileId { get; set; }            // đổi kiểu nếu NguoiDung.Id không phải int

        public byte LoaiGiayTo { get; set; }

        [Required, MaxLength(500)]
        public string SoGiayTo { get; set; } = string.Empty;   // bản mã hóa

        [MaxLength(32)]
        public byte[]? SoGiayToHash { get; set; }              // HMAC-SHA256, BINARY(32)

        [MaxLength(200)]
        public string? HoTenTrenGiayTo { get; set; }

        public DateTime? NgayCap { get; set; }
        public DateTime? NgayHetHan { get; set; }

        [MaxLength(200)]
        public string? NoiCap { get; set; }

        [MaxLength]
        public string? AnhMatTruoc { get; set; }

        [MaxLength]
        public string? AnhMatSau { get; set; }

        public byte TrangThaiXacMinh { get; set; }

        [MaxLength(100)]
        public string? NguoiXacMinh { get; set; }

        public DateTime? NgayXacMinh { get; set; }

        [MaxLength(500)]
        public string? GhiChuXacMinh { get; set; }

        public bool LaGiayToChinh { get; set; } = true;
        public bool IsDel { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? NguoiTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        [MaxLength(100)]
        public string? NguoiCapNhat { get; set; }

        // Navigation
        [ForeignKey(nameof(UserProfileId))]
        public NguoiDung NguoiDung { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;

namespace Booking.Service.Dtos.Hotels
{
    public class UpdateHotelRequest
    {
        [Required, StringLength(200)]
        public string TenKhachSan { get; set; } = string.Empty;

        [StringLength(4000)]
        public string? MoTa { get; set; }

        [StringLength(2000)]
        public string? ChinhSachHuy { get; set; }

        [Required, StringLength(500)]
        public string DiaChi { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string ThanhPho { get; set; } = string.Empty;

        public double? ViDo { get; set; }
        public double? KinhDo { get; set; }

        [Range(1, 5)]
        public int SoSao { get; set; }

        [Required]
        public string GioNhanPhong { get; set; } = string.Empty;

        [Required]
        public string GioTraPhong { get; set; } = string.Empty;
    }
}
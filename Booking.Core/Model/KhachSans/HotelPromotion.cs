using System.ComponentModel.DataAnnotations;

namespace Booking.Core.Model.KhachSans
{
    public class HotelPromotion
    {
        [Key]
        public Guid Id { get; set; }

        public Guid HotelId { get; set; }

        [Required, MaxLength(40)]
        public string Code { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string DiscountType { get; set; } = string.Empty;

        public decimal DiscountValue { get; set; }
        public decimal? MinBookingAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual KhachSan Hotel { get; set; } = null!;
    }
}
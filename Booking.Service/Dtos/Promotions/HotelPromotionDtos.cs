using System.ComponentModel.DataAnnotations;

namespace Booking.Service.Dtos.Promotions
{
    public class SaveHotelPromotionRequest
    {
        [Required, StringLength(40)]
        public string Code { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string DiscountType { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "1000000000")]
        public decimal DiscountValue { get; set; }

        [Range(typeof(decimal), "0", "1000000000")]
        public decimal? MinBookingAmount { get; set; }

        [Range(typeof(decimal), "0.01", "1000000000")]
        public decimal? MaxDiscountAmount { get; set; }

        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
    }

    public class HotelPromotionResponse
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal? MinBookingAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public bool IsActive { get; set; }
    }
}
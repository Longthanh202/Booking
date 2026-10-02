using Booking.Service.Dtos.Promotions;

namespace Booking.Service.Services.Promotions
{
    public class PromotionNotApplicableException : Exception
    {
        public PromotionNotApplicableException() : base("Mã khuyến mãi không hợp lệ, chưa đến hạn, đã hết hạn hoặc không đạt điều kiện đơn hàng.")
        {
        }
    }

    public class PromotionApplication
    {
        public Guid PromotionId { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
    }

    public interface IHotelPromotionService
    {
        Task<List<HotelPromotionResponse>?> GetForOwner(Guid ownerId, Guid hotelId);
        Task<HotelPromotionResponse?> Create(Guid ownerId, Guid hotelId, SaveHotelPromotionRequest request);
        Task<HotelPromotionResponse?> Update(Guid ownerId, Guid promotionId, SaveHotelPromotionRequest request);
        Task<bool> Deactivate(Guid ownerId, Guid promotionId);
        Task<PromotionApplication?> ApplyForBooking(Guid hotelId, string code, decimal subtotal);
    }
}
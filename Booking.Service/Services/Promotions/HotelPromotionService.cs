using System.Text.RegularExpressions;
using Booking.Core.Model.KhachSans;
using Booking.Data.DBContext;
using Booking.Service.Dtos.Promotions;
using Microsoft.EntityFrameworkCore;

namespace Booking.Service.Services.Promotions
{
    public class HotelPromotionService : IHotelPromotionService
    {
        private static readonly Regex ValidCode = new("^[A-Z0-9]+(?:-[A-Z0-9]+)*$", RegexOptions.Compiled);
        private readonly AppDbContext _context;

        public HotelPromotionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<HotelPromotionResponse>?> GetForOwner(Guid ownerId, Guid hotelId)
        {
            if (!await OwnsHotel(ownerId, hotelId)) return null;
            var items = await _context.HotelPromotions.AsNoTracking()
                .Where(promotion => promotion.HotelId == hotelId)
                .OrderByDescending(promotion => promotion.CreatedAt)
                .ToListAsync();
            return items.Select(Map).ToList();
        }

        public async Task<HotelPromotionResponse?> Create(Guid ownerId, Guid hotelId, SaveHotelPromotionRequest request)
        {
            if (!await OwnsHotel(ownerId, hotelId) || !IsValid(request)) return null;
            var code = request.Code.Trim().ToUpperInvariant();
            if (await _context.HotelPromotions.AnyAsync(promotion =>
                promotion.HotelId == hotelId && promotion.Code == code)) return null;

            var promotion = new HotelPromotion
            {
                Id = Guid.NewGuid(),
                HotelId = hotelId,
                Code = code,
                Name = request.Name.Trim(),
                DiscountType = request.DiscountType.Trim().ToUpperInvariant(),
                DiscountValue = request.DiscountValue,
                MinBookingAmount = request.MinBookingAmount,
                MaxDiscountAmount = request.MaxDiscountAmount,
                StartsAt = request.StartsAt,
                EndsAt = request.EndsAt,
                IsActive = true,
                CreatedBy = ownerId,
                CreatedAt = DateTime.UtcNow
            };
            await _context.HotelPromotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
            return Map(promotion);
        }

        public async Task<HotelPromotionResponse?> Update(Guid ownerId, Guid promotionId, SaveHotelPromotionRequest request)
        {
            var promotion = await _context.HotelPromotions.FirstOrDefaultAsync(item =>
                item.Id == promotionId && item.Hotel.NguoiTao == ownerId);
            if (promotion == null || !IsValid(request)) return null;

            var code = request.Code.Trim().ToUpperInvariant();
            if (await _context.HotelPromotions.AnyAsync(item =>
                item.HotelId == promotion.HotelId && item.Id != promotionId && item.Code == code)) return null;

            promotion.Code = code;
            promotion.Name = request.Name.Trim();
            promotion.DiscountType = request.DiscountType.Trim().ToUpperInvariant();
            promotion.DiscountValue = request.DiscountValue;
            promotion.MinBookingAmount = request.MinBookingAmount;
            promotion.MaxDiscountAmount = request.MaxDiscountAmount;
            promotion.StartsAt = request.StartsAt;
            promotion.EndsAt = request.EndsAt;
            await _context.SaveChangesAsync();
            return Map(promotion);
        }

        public async Task<bool> Deactivate(Guid ownerId, Guid promotionId)
        {
            var promotion = await _context.HotelPromotions.FirstOrDefaultAsync(item =>
                item.Id == promotionId && item.Hotel.NguoiTao == ownerId);
            if (promotion == null) return false;
            promotion.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PromotionApplication?> ApplyForBooking(Guid hotelId, string code, decimal subtotal)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();
            var now = DateTime.UtcNow;
            var promotion = await _context.HotelPromotions.AsNoTracking().FirstOrDefaultAsync(item =>
                item.HotelId == hotelId && item.Code == normalizedCode && item.IsActive &&
                item.StartsAt <= now && item.EndsAt > now);
            if (promotion == null || (promotion.MinBookingAmount.HasValue && subtotal < promotion.MinBookingAmount.Value))
            {
                return null;
            }

            var discount = promotion.DiscountType == "PERCENT"
                ? subtotal * promotion.DiscountValue / 100m
                : promotion.DiscountValue;
            if (promotion.MaxDiscountAmount.HasValue)
            {
                discount = Math.Min(discount, promotion.MaxDiscountAmount.Value);
            }

            return new PromotionApplication
            {
                PromotionId = promotion.Id,
                Code = promotion.Code,
                DiscountAmount = Math.Min(subtotal, Math.Round(discount, 2, MidpointRounding.AwayFromZero))
            };
        }

        private Task<bool> OwnsHotel(Guid ownerId, Guid hotelId) =>
            _context.KhachSans.AnyAsync(hotel => hotel.Id == hotelId && hotel.NguoiTao == ownerId);

        private static bool IsValid(SaveHotelPromotionRequest request)
        {
            var code = request.Code.Trim().ToUpperInvariant();
            var type = request.DiscountType.Trim().ToUpperInvariant();
            return !string.IsNullOrWhiteSpace(request.Name) &&
                ValidCode.IsMatch(code) &&
                (type == "PERCENT" ? request.DiscountValue <= 100 : type == "FIXED") &&
                request.StartsAt < request.EndsAt &&
                (!request.MinBookingAmount.HasValue || request.MinBookingAmount >= 0) &&
                (!request.MaxDiscountAmount.HasValue || request.MaxDiscountAmount > 0);
        }

        private static HotelPromotionResponse Map(HotelPromotion promotion) => new()
        {
            Id = promotion.Id,
            HotelId = promotion.HotelId,
            Code = promotion.Code,
            Name = promotion.Name,
            DiscountType = promotion.DiscountType,
            DiscountValue = promotion.DiscountValue,
            MinBookingAmount = promotion.MinBookingAmount,
            MaxDiscountAmount = promotion.MaxDiscountAmount,
            StartsAt = promotion.StartsAt,
            EndsAt = promotion.EndsAt,
            IsActive = promotion.IsActive
        };
    }
}
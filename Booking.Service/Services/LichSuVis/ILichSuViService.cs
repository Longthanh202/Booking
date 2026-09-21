using Booking.Service.Dtos.Wallet;

namespace Booking.Service.Services.LichSuVis;

public interface ILichSuViService
{
    Task<List<WalletHistoryDto>> GetOwnerWalletHistory(Guid ownerId);
}
using Booking.Service.Dtos.LichSuVi;

namespace Booking.Service.Services.LichSuVis;

public interface ILichSuViService
{
    Task<List<LichSuViDto>> GetOwnerWalletHistory(Guid ownerId);
}
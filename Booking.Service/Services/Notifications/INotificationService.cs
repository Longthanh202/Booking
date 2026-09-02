using Booking.Core.Model.DatPhongs;

namespace Booking.Service.Services.Notifications
{
    public interface INotificationService
    {
        Task BookingSuccess(Guid userId, DatPhong booking);
    }
}


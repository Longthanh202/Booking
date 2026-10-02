using Booking.Core.Model.DatPhongs;

namespace Booking.Service.Services.Notifications
{
    public interface INotificationService
    {
        Task BookingSuccess(Guid userId, DatPhong booking);
        Task BookingCreatedForOwner(Guid ownerId, DatPhong booking);
        Task BookingStatusChanged(Guid recipientId, DatPhong booking, string message);
    }
}


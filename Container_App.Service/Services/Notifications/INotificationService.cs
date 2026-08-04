using Container_App.Core.Model.DatPhongs;

namespace Container_App.Service.Services.Notifications
{
    public interface INotificationService
    {
        Task BookingSuccess(Guid userId, DatPhong booking);
    }
}


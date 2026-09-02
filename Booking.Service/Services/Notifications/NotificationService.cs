using Booking.Core.Model.DatPhongs;
using Booking.Service.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Booking.Service.Services.Notifications;

public class NotificationService: INotificationService
{
    private readonly IHubContext<BookingHub> _hubContext;

    public NotificationService(
        IHubContext<BookingHub> hubContext)
    {
        _hubContext = hubContext;
    }
    public async Task BookingSuccess(Guid userId, DatPhong booking)
    {
        await _hubContext
            .Clients
            .User(userId.ToString())
            .SendAsync("BookingSuccess", new
            {
                booking.Id,
                booking.TongTien,
                booking.TrangThai,
                Message = "Đặt phòng thành công"
            });
    }
}
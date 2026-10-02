using Booking.Core.Model.DatPhongs;
using Booking.Service.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Booking.Service.Services.Notifications
{



    public class NotificationService : INotificationService
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

        public Task BookingCreatedForOwner(Guid ownerId, DatPhong booking)
        {
            return _hubContext.Clients.User(ownerId.ToString()).SendAsync("OwnerBookingCreated", new
            {
                booking.Id,
                booking.KhachSanId,
                booking.NgayNhanPhong,
                booking.NgayTraPhong,
                booking.TongTien,
                booking.TrangThai,
                Message = "Có booking mới cần xử lý"
            });
        }

        public Task BookingStatusChanged(Guid recipientId, DatPhong booking, string message)
        {
            return _hubContext.Clients.User(recipientId.ToString()).SendAsync("BookingStatusChanged", new
            {
                booking.Id,
                booking.KhachSanId,
                booking.TrangThai,
                Message = message
            });
        }
    }
}
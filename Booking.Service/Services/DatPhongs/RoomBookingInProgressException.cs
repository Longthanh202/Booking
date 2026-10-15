namespace Booking.Service.Services.DatPhongs
{
    public sealed class RoomBookingInProgressException : Exception
    {
        public RoomBookingInProgressException()
            : base("Phòng đang được người khác đặt. Vui lòng thử lại.")
        {
        }
    }
}

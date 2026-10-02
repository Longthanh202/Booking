namespace Booking.Service.Dtos.Rooms
{
    public class CreateRoomPriceRequest
    {
        public Guid LoaiPhongId { get; set; }
        public decimal Gia { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
    }
}
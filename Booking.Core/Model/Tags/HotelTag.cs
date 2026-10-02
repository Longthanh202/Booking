using Booking.Core.Model.KhachSans;

namespace Booking.Core.Model.Tags
{
    public class HotelTag
    {
        public Guid HotelId { get; set; }
        public Guid TagId { get; set; }

        public virtual KhachSan Hotel { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
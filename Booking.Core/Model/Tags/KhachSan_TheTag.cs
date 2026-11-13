using Booking.Core.Model.KhachSans;

namespace Booking.Core.Model.Tags
{
    public class KhachSan_TheTag
    {
        public Guid HotelId { get; set; }
        public Guid TagId { get; set; }

        public virtual KhachSan Hotel { get; set; } = null!;
        public virtual TheTag TheTag { get; set; } = null!;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.HotelImages
{
    public class HotelImageDto
    {
        public long Id { get; set; }
        public string? Url { get; set; } = string.Empty;
    }
}

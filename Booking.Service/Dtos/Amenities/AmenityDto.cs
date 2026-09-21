using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.Amenities
{
    public class AmenityDto
    {
        public Guid Id { get; set; }
        public string? TenTienIch { get; set; } = string.Empty;
        public string? Icon { get; set; } = string.Empty;
    }
}

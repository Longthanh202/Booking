using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.Amenities
{
    public class CreateAmenityRequest
    {
        public string? TenTienIch { get; set; }
        public string? Icon { get; set; }
        public Guid KhachSanId { get; set; }
    }
}

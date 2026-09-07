using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.TienIchs
{
    public class TienIchRequest
    {
        public string? TenTienIch { get; set; }
        public string? Icon { get; set; }
        public Guid KhachSanId { get; set; }
    }
}

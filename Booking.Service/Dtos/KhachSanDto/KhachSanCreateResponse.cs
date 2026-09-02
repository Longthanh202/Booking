using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Core.Model.KhachSans;

namespace Booking.Service.Dtos.KhachSanDto
{
    public class KhachSanCreateResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public Booking.Core.Model.KhachSans.KhachSan Data { get; set; }
    }
}

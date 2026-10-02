using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Core.Model.KhachSans;

namespace Booking.Service.Dtos.Hotels
{
    public class CreateHotelResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public Booking.Core.Model.KhachSans.KhachSan Data { get; set; }
    }
}

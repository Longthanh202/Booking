using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.Authentication
{
    public class LoginResponse
    {
        public bool Status { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.Login
{
    public class LoginReponse
    {
        public bool status { get; set; }
        public string token { get; set; }
        public string message { get; set; }
    }
}

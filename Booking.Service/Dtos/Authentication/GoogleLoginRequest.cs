using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.Authentication
{
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; } = string.Empty;
    }
}

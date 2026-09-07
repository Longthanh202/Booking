using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.UserProfile
{
    public class UserProfileResponse
    {
        public string FullName { get; set; }

        public List<string> Permissions { get; set; }
    }
}

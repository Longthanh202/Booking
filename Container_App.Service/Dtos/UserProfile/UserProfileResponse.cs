using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.UserProfile
{
    public class UserProfileResponse
    {
        public string FullName { get; set; }

        public List<string> Permissions { get; set; }
    }
}

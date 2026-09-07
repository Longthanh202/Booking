using Booking.Core.Model.UserRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.Users
{
    public class UserLogin
    {
        [Key]
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public virtual UserProfile? UserProfile { get; set; }
        public virtual UserRole? UserRole { get; set; }
    }
}

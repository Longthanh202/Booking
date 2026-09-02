using Booking.Core.Model.Roles;
using Booking.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.UserRoles
{
    public class UserRole
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        // Navigations
        public virtual UserProfile? UserProfile { get; set; }
        public virtual UserLogin? UserLogin { get; set; }
        public virtual Role? Role { get; set; }
    }
}

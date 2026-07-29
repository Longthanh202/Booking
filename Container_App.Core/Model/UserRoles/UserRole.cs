using Container_App.Core.Model.Roles;
using Container_App.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.UserRoles
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

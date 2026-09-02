using Booking.Core.Model.Permissions;
using Booking.Core.Model.Resources;
using Booking.Core.Model.Roles;
using Booking.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.RolePermissions
{
    public class RolePermission
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? ResourceId { get; set; }
        public Guid? PermissionId { get; set; }

        // Navigations
        public virtual Role? Role { get; set; }
        public virtual Booking.Core.Model.Resources.Resources? Resource { get; set; }
        public virtual Permission? Permission { get; set; }
    }
}

using Container_App.Core.Model.Permissions;
using Container_App.Core.Model.Resources;
using Container_App.Core.Model.Roles;
using Container_App.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.RolePermissions
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
        public virtual Container_App.Core.Model.Resources.Resources? Resource { get; set; }
        public virtual Permission? Permission { get; set; }
    }
}

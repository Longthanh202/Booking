using Container_App.Core.Model.RolePermissions;
using Container_App.Core.Model.UserRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.Roles
{
    public class Role
    {
        public Guid Id { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateAt { get; set; }
        public Guid? CreateBy { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
        public virtual ICollection<UserRole> UserRoles { get; set; }
        = new List<UserRole>();
    }
}

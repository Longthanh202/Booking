using Container_App.Core.Model.RolePermissions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.Permissions
{
    public class Permission
    {
        [Key]
        public Guid Id { get; set; }
        public string? Action { get; set; }
        public DateTime? CreateAt { get; set; }
        public Guid? CreateBy { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}

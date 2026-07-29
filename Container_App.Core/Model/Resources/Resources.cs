using Container_App.Core.Model.RolePermissions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.Resources
{
    public class Resources
    {
        [Key]
        public Guid Id { get; set; }
        public string? ResourceName { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; } 
        = new List<RolePermission>();
    }
}

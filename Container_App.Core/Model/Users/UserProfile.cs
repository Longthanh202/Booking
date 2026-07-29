using Container_App.Core.Model.RolePermissions;
using Container_App.Core.Model.UserRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.Users
{
    
    public class UserProfile
    {
        [Key]
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? IsDel { get; set; }
        public DateTime? CreateAt { get; set; }
        public Guid? CreateBy { get; set; }
        public Guid? UserLoginId { get; set; }
        [NotMapped]
        public Guid RoleId { get; set; }
        [NotMapped]
        public string? RoleName { get; set; }

        public virtual UserLogin? UserLogin { get; set; }
        public virtual UserRole? UserRole { get; set; }
    }
}

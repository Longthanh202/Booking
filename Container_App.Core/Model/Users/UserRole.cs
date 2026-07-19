using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.Users
{
    public class UserRole
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserProfile User { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public UserLogin UserLogin { get; set; }
    }
}

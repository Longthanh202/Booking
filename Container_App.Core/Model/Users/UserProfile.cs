using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.Users
{
    public class UserLogin
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class UserProfile : UserLogin
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int IsDel { get; set; }
        public DateTime? CreateAt { get; set; }
        public Guid? CreateBy { get; set; }
        public Guid UserLoginId { get; set; }
        public Guid RoleId { get; set; }
        public string? RoleName { get; set; }
    }

    public class Role
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public Guid UserId { get; set; }
    }
}

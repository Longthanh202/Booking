using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.Users
{
    public class ExternalLogin
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserLoginId { get; set; }
        public virtual UserLogin? UserLogin { get; set; }

        public string Provider { get; set; } = string.Empty;
        public string ProviderKey { get; set; } = string.Empty;

        // Thêm thuộc tính lưu URL Avatar
        public string? Avatar { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

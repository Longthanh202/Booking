using Booking.Core.Model.Roles;
using Booking.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.UserRoles
{
    public class NguoiDung_VaiTro
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        // Navigations
        public virtual NguoiDung? NguoiDung { get; set; }
        public virtual TaiKhoan? TaiKhoan { get; set; }
        public virtual VaiTro? VaiTro { get; set; }
    }
}

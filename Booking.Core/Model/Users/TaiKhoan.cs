using Booking.Core.Model.UserRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.Users
{
    public class TaiKhoan
    {
        [Key]
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public virtual NguoiDung? NguoiDung { get; set; }
        public virtual NguoiDung_VaiTro? NguoiDung_VaiTro { get; set; }
    }
}

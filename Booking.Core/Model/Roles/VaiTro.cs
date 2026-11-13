using Booking.Core.Model.UserRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.Roles
{
    public class VaiTro
    {
        public Guid Id { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateAt { get; set; }
        public Guid? CreateBy { get; set; }
        public virtual ICollection<NguoiDung_VaiTro> NguoiDung_VaiTros { get; set; }
        = new List<NguoiDung_VaiTro>();
    }
}

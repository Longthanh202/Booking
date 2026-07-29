using Container_App.Core.Model.KhachSans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.DanhGias
{
    public class DanhGia
    {
        public Guid Id { get; set; }
        public Guid? KhachSanId { get; set; }
        public Guid? KhachHangId { get; set; }
        public int? SoSao { get; set; }
        public string? NoiDung { get; set; }
        public DateTime? NgayTao { get; set; }

        // Navigation
        public virtual KhachSan? KhachSan { get; set; }
    }
}

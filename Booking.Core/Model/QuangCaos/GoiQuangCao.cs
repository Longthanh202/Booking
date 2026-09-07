using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.QuangCaos
{
    public class GoiQuangCao
    {
        public int Id { get; set; }
        public string? TenGoi { get; set; }
        public int? SoNgay { get; set; }
        public decimal? GiaTien { get; set; }
        public int? DiemUuTien { get; set; }
        public int? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        // Navigation
        public virtual ICollection<KhachSanQuangCao> KhachSanQuangCaos { get; set; } = new List<KhachSanQuangCao>();
    }
}

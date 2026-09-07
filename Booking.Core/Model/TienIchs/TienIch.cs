using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.TienIchs
{
    public class TienIch
    {
        public Guid Id { get; set; }
        public string? TenTienIch { get; set; }
        public string? Icon { get; set; }

        // Navigation Nhiều - Nhiều
        public virtual ICollection<KhachSan_TienIch> KhachSan_TienIches { get; set; } = new List<KhachSan_TienIch>();
    }
}

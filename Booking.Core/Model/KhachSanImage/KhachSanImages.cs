using Booking.Core.Model.KhachSans;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.KhachSanImage
{
    public class KhachSanImages
    {
        [Key]
        public long Id { get; set; }
        public Guid KhachSanId { get; set; }
        public string? Url { get; set; }
        public virtual KhachSan? KhachSan { get; set; }
    }
}

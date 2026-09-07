using Booking.Core.Model.LoaiPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.GiaPhongs
{
    public class GiaPhong
    {
        public Guid Id { get; set; }
        public Guid LoaiPhongId { get; set; }
        public decimal Gia { get; set; } // decimal(18,2) -> decimal
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public bool IsActive { get; set; } // bit -> bool
        public DateTime? NgayTao { get; set; }

        // Navigation
        public virtual LoaiPhong? LoaiPhong { get; set; }
    }
}

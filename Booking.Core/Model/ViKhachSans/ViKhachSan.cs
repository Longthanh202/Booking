using Booking.Core.Model.KhachSans;
using Booking.Core.Model.LichSuVis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.ViKhachSans
{
    public class ViKhachSan
    {
        public long MaVi { get; set; }

        public Guid MaKhachSan { get; set; }

        public decimal SoDu { get; set; }

        public decimal SoDuTamGiu { get; set; }

        public string TrangThai { get; set; }

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        #region Navigation

        public virtual KhachSan KhachSan { get; set; }

        public virtual ICollection<LichSuVi> LichSuVis { get; set; } = new List<LichSuVi>();

        #endregion
    }
}

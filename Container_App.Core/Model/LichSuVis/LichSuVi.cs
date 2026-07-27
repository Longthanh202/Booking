using Container_App.Core.Model.DatPhongs;
using Container_App.Core.Model.ViKhachSans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.LichSuVis
{
    public class LichSuVi
    {
        public long MaLichSu { get; set; }

        public long MaVi { get; set; }

        public Guid? MaDatPhong { get; set; }

        public decimal SoTien { get; set; }

        public string LoaiGiaoDich { get; set; }

        public string? NoiDung { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        #region Navigation

        public virtual ViKhachSan ViKhachSan { get; set; }

        public virtual DatPhong? DatPhong { get; set; }

        #endregion
    }
}

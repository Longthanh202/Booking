using Container_App.Core.Model.ChiTraKhachSans;
using Container_App.Core.Model.DatPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.ChiTietChiTraKhachSans
{
    public class ChiTietChiTraKhachSan
    {
        public long Id { get; set; }

        public long ChiTraKhachSanId { get; set; }

        public Guid DatPhongId { get; set; }

        public decimal SoTienThanhToan { get; set; }

        public decimal SoTienHoaHong { get; set; }

        public decimal SoTienThucNhan { get; set; }

        #region Navigation

        public virtual ChiTraKhachSan ChiTraKhachSan { get; set; }

        public virtual DatPhong DatPhong { get; set; }

        #endregion
    }
}

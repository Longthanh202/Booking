using Container_App.Core.Model.HoaDonHoaHongs;
using Container_App.Core.Model.HoaHongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.ChiTietHoaDonHoaHongs
{
    public class ChiTietHoaDonHoaHong
    {
        public long Id { get; set; }

        public long HoaDonHoaHongId { get; set; }

        public long HoaHongId { get; set; }

        #region Navigation

        public virtual HoaDonHoaHong HoaDonHoaHong { get; set; }

        public virtual HoaHong HoaHong { get; set; }

        #endregion
    }
}

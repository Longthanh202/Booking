using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.LoaiPhongs
{
    public class LoaiPhongHienThi
    {
        public Guid Id { get; set; }

        public string TenLoaiPhong { get; set; }

        public int SoKhachToiDa { get; set; }

        public string KieuGiuong { get; set; }

        public string MoTa { get; set; }

        public decimal GiaMoiDem { get; set; }

        public decimal TongTien { get; set; }

        public int SoPhongTrong { get; set; }

        public bool DuChoSoKhach { get; set; }
    }
}

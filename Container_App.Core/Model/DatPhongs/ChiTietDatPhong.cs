using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.DatPhongs
{
    public class ChiTietDatPhong
    {
        [Key]
        public Guid Id { get; set; }

        public Guid DatPhongId { get; set; }

        public Guid LoaiPhongId { get; set; }

        public int SoLuongPhong { get; set; }

        public decimal GiaMoiDem { get; set; }

        public DatPhong? DatPhong { get; set; } = null!;
    }
}

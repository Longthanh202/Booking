using Container_App.Core.Model.DatPhongs;
using Container_App.Core.Model.Phongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.PhongDats
{
    public class PhongDat
    {
        public Guid Id { get; set; }
        public Guid ChiTietDatPhongId { get; set; }
        public Guid PhongId { get; set; }
        public string? TrangThai {  get; set; }

        public ChiTietDatPhong? ChiTietDatPhong { get; set; }
        public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();

    }
}

using Container_App.Core.Model.DatPhongs;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.PhongDats;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.Phongs
{
    public class Phong
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? LoaiPhongId { get; set; }
        public string? SoPhong { get; set; }
        public int? Tang { get; set; }
        public string? TrangThai { get; set; }

        // Navigations
        public virtual LoaiPhong? LoaiPhong { get; set; }
        public virtual ICollection<PhongDat> PhongDats { get; set; } = new List<PhongDat>();
    }
}

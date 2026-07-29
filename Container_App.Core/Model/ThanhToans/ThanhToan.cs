using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.ThanhToans
{
    public class ThanhToan
    {
        public Guid Id { get; set; }
        public Guid DatPhongId { get; set; }
        public string? PhuongThuc { get; set; }
        public decimal? SoTien { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? ThoiGianThanhToan { get; set; }
    }
}

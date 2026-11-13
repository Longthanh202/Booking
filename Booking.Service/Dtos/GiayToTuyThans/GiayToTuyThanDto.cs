using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.GiayToTuyThans
{
    public class GiayToTuyThanDto
    {
        public long Id { get; set; }
        public Guid UserProfileId { get; set; }
        public byte LoaiGiayTo { get; set; }
        public string SoGiayToDaChe { get; set; } = string.Empty;   // ví dụ ********6789
        public string? HoTenTrenGiayTo { get; set; }
        public DateTime? NgayCap { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string? NoiCap { get; set; }
        public string? AnhMatTruoc { get; set; }
        public string? AnhMatSau { get; set; }
        public byte TrangThaiXacMinh { get; set; }
        public string? NguoiXacMinh { get; set; }
        public DateTime? NgayXacMinh { get; set; }
        public string? GhiChuXacMinh { get; set; }
        public bool LaGiayToChinh { get; set; }
        public DateTime NgayTao { get; set; }
    }
}

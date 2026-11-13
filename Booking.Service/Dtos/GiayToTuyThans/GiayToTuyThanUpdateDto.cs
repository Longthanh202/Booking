using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.GiayToTuyThans
{
    public class GiayToTuyThanUpdateDto
    {
        [Range(1, 3)]
        public byte LoaiGiayTo { get; set; }

        [Required, StringLength(50, MinimumLength = 6)]
        public string SoGiayTo { get; set; } = string.Empty;

        [StringLength(200)] public string? HoTenTrenGiayTo { get; set; }
        public DateTime? NgayCap { get; set; }
        public DateTime? NgayHetHan { get; set; }
        [StringLength(200)] public string? NoiCap { get; set; }
        [StringLength(500)] public string? AnhMatTruoc { get; set; }
        [StringLength(500)] public string? AnhMatSau { get; set; }
    }
}

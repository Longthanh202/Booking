using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.KhachSanDto
{
    public class AdminFilterHotelRequestDto
    {
        public string? Keyword { get; set; }
        public string? ThanhPho { get; set; }
        public double? ViDo { get; set; } = 0;
        public double? KinhDo { get; set; } = 0;
        public int SoSao { get; set; } = 0;
        public string? TrangThai { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

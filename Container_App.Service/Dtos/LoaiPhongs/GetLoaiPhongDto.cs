using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.LoaiPhongs
{
    public class GetLoaiPhongDto
    {
        public Guid khachSanId { get; set; }
        public int soKhach { get; set; }
        public DateTime? ngayNhan { get; set; }
        public DateTime? ngayTra { get; set; }       
    }
}

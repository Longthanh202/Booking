using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.KhachSanDto
{
    public class OwnerFilterHotelRequestDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
    }
}

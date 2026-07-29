using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.TienIchs
{
    public class TienIchDto
    {
        public Guid Id { get; set; }
        public string? TenTienIch { get; set; } = string.Empty;
        public string? Icon { get; set; } = string.Empty;
    }
}

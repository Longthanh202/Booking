using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.TienIchs
{
    public class TienIch
    {
        [Key]
        public Guid Id { get; set; }
        public string TenTienIch { get; set; }
        public string Icon { get; set; }
        public ICollection<KhachSan_TienIch> KhachSan_TienIchs { get; set; }
       = new List<KhachSan_TienIch>();
    }
}

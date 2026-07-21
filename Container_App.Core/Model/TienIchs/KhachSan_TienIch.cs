using Container_App.Core.Model.KhachSans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.TienIchs
{
    public class KhachSan_TienIch
    {
        public Guid KhachSanId { get; set; }
        public virtual KhachSan? KhachSan { get; set; }

        public Guid TienIchId { get; set; }
        public virtual TienIch? TienIch { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container_App.Core.Model.KhachSans;

namespace Container_App.Service.Dtos.KhachSanDto
{
    public class KhachSanCreateReponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public Container_App.Core.Model.KhachSans.KhachSan Data { get; set; }
    }
}

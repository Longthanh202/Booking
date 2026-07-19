using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.Banner
{
    public class InsertBannerDto
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int IsActive { get; set; }
        public IFormFile File { get; set; }
    }
}

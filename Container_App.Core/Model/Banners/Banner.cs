using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.Banners
{
    public class Banner
    {
        [Key]
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Url { get; set; }
        public int? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

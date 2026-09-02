using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Common.Config
{
    public enum StatusProject
    {
        Active = 1,
        Lock = 2,
        Cancel = 3,
    }

    public enum StatusTask
    {
        Active = 1,
        Complete = 2,
        Cancel = 3,
    }
}

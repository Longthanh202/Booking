using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Common.Shared
{
    public static class Paginations
    {
        public static int GetStartRow(int page, int pageSize)
        {
            return (page - 1) * pageSize + 1;
        }

        public static int GetEndRow(int page, int pageSize)
        {
            return page * pageSize;
        }

        public static int GetTotalPages(int totalItems, int pageSize)
        {
            return (int)Math.Ceiling((double)totalItems / pageSize);
        }
    }
}

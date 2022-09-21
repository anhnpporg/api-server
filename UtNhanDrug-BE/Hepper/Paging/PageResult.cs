using System.Collections.Generic;

namespace UtNhanDrug_BE.Hepper.Paging
{
    public class PageResult<T> : PagedResultBase
    {
        public List<T> Items { get; set; }
        public int TotalRecord { get; set; }
    }
}

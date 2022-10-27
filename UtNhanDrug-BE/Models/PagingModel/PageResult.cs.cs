using System.Collections.Generic;

namespace UtNhanDrug_BE.Models.PagingModel
{
    public class PageResult<T> : PagedResultBase
    {
        public List<T> Items { get; set; }
    }
}

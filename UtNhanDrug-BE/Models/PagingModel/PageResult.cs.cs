using System.Collections.Generic;

namespace UtNhanDrug_BE.Models.PagingModel
{
    public class PageResult<T> : PagedResultBase
    {
        public int StatusCode { get; set; }
        public List<T> Items { get; set; }
        public string Message { get; set; }
    }
}

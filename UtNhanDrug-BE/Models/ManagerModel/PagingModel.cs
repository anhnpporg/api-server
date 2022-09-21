using UtNhanDrug_BE.Hepper.Paging;

namespace UtNhanDrug_BE.Models.ManagerModel
{
    public class PagingModel : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}

using UtNhanDrug_BE.Models.PagingModel;

namespace UtNhanDrug_BE.Models.ProductModel
{
    public class ProductFilterRequest : PagingRequestBase
    {
        public string SearchValue { get; set; }
    }
}

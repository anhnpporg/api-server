using UtNhanDrug_BE.Models.PagingModel;

namespace UtNhanDrug_BE.Models.UserModel
{
    public class CustomerPagingRequest : PagingRequestBase
    {
        public string PhoneNumber { get; set; }
    }
}

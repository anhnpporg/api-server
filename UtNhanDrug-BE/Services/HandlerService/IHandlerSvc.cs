using System.Threading.Tasks;

namespace UtNhanDrug_BE.Services.HandlerService
{
    public interface IHandlerSvc
    {
        Task CheckExpiryBatch();
        Task CheckExpiryBatch1();
        Task CheckQuantityOfProduct(int productId);
        Task CheckQuantityOfProduct();
        
    }
}

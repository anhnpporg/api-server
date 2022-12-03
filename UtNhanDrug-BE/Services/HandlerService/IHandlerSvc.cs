using System.Threading.Tasks;

namespace UtNhanDrug_BE.Services.HandlerService
{
    public interface IHandlerSvc
    {
        Task CheckExpiryBatch();
        Task CheckQuantityOfProduct(int productId);
    }
}

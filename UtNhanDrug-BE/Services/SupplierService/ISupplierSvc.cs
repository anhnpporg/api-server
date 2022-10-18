using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.SupplierModel;

namespace UtNhanDrug_BE.Services.SupplierService
{
    public interface ISupplierSvc
    {
        Task<bool> CreateSupplier(int userId, CreateSupplierModel model);
        Task<bool> UpdateSupplier(int supplierId, int userId, UpdateSupplierModel model);
        Task<bool> DeleteSupplier(int supplierId, int userId);
        Task<ViewSupplierModel> GetSupplierById(int supplierId);
        Task<List<ViewSupplierModel>> GetAllSupplier();
        Task<bool> CheckSupplier(int supplierId);
        Task<List<ViewProductModel>> GetListProduct(int supplierId);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.BatchModel;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.ResponseModel;
using UtNhanDrug_BE.Models.SupplierModel;

namespace UtNhanDrug_BE.Services.SupplierService
{
    public interface ISupplierSvc
    {
        Task<Response<bool>> CreateSupplier(int userId, CreateSupplierModel model);
        Task<Response<bool>> UpdateSupplier(int supplierId, int userId, UpdateSupplierModel model);
        Task<Response<bool>> IsDeleteSupplier(int supplierId, int userId);
        Task<Response<ViewSupplierModel>> GetSupplierById(int supplierId);
        Task<Response<List<ViewSupplierModel>>> GetAllSupplier();
        Task<Response<List<ViewModel>>> GetListSupplier();
        //Task<Response<bool>> CheckSupplier(int supplierId);
        Task<Response<List<ViewProductModel>>> GetListProduct(int supplierId);
        Task<Response<List<ViewBatchModel>>> GetListBatch(int supplierId);
    }
}

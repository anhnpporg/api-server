using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ProductUnitModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.ProductUnitService
{
    public interface IProductUnitPriceSvc
    {
        Task<Response<bool>> CreateProductUnit(int userId, CreateProductUnitPriceModel model);
        Task<Response<bool>> UpdateProductUnit(int productUnitId, int userId, UpdateProductUnitPriceModel model);
        //Task<bool> DeleteProductUnit(int productUnitId, int userId);
        Task<Response<ViewProductUnitPriceModel>> GetProductUnitById(int productUnitId);
        Task<Response<List<ViewProductUnitPriceModel>>> GetProductUnitByProductId(int productId);
        //Task<Response<bool>> CheckProductUnit(int productUnitId);
    }
}

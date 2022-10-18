using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ProductUnitModel;

namespace UtNhanDrug_BE.Services.ProductUnitService
{
    public interface IProductUnitSvc
    {
        Task<bool> CreateProductUnit(CreateProductUnitModel model);
        Task<bool> UpdateProductUnit(int productUnitId, UpdateProductUnitModel model);
        //Task<bool> DeleteProductUnit(int productUnitId, int userId);
        Task<ViewProductUnitModel> GetProductUnitById(int productUnitId);
        Task<List<ViewProductUnitModel>> GetProductUnitByProductId(int productId);
        Task<bool> CheckProductUnit(int productUnitId);
    }
}

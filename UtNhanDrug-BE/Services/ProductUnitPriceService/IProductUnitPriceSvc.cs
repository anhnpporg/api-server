using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ProductUnitModel;

namespace UtNhanDrug_BE.Services.ProductUnitService
{
    public interface IProductUnitPriceSvc
    {
        Task<bool> CreateProductUnit(int userId, CreateProductUnitPriceModel model);
        Task<bool> UpdateProductUnit(int productUnitId, int userId, UpdateProductUnitPriceModel model);
        //Task<bool> DeleteProductUnit(int productUnitId, int userId);
        Task<ViewProductUnitPriceModel> GetProductUnitById(int productUnitId);
        Task<List<ViewProductUnitPriceModel>> GetProductUnitByProductId(int productId);
        Task<bool> CheckProductUnit(int productUnitId);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ProductModel;

namespace UtNhanDrug_BE.Services.ProductService
{
    public interface IProductSvc
    {
        Task<bool> CreateProduct(int userId, CreateProductModel model);
        Task<bool> UpdateProduct(int brandId, int userId, UpdateProductModel model);
        Task<bool> DeleteProduct(int brandId, int userId);
        Task<ViewProductModel> GetProductById(int brandId);
        Task<List<ViewProductModel>> GetAllProduct();
        Task<bool> CheckProduct(int brandId);
    }
}

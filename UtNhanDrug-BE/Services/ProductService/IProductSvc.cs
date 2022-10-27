using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.PagingModel;
using UtNhanDrug_BE.Models.ProductModel;

namespace UtNhanDrug_BE.Services.ProductService
{
    public interface IProductSvc
    {
        Task<bool> CreateProduct(int userId, CreateProductModel model);
        Task<bool> UpdateProduct(int brandId, int userId, UpdateProductModel model);
        Task<bool> DeleteProduct(int brandId, int userId);
        Task<ViewProductModel> GetProductById(int id);
        Task<List<ViewProductModel>> GetAllProduct();
        Task<bool> CheckProduct(int brandId);
        Task<List<ViewModel>> GetListActiveSubstances(int productId);
        Task<List<ViewModel>> GetListRouteOfAdmin();
        Task<List<ViewModel>> GetListStockStrengthUnits();
        Task<PageResult<ViewProductModel>> GetProductPaging(ProductPagingRequest request);
    }
}

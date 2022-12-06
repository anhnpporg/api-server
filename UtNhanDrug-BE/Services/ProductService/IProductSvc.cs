using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ActiveSubstanceModel;
using UtNhanDrug_BE.Models.BatchModel;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.PagingModel;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.ProductService
{
    public interface IProductSvc
    {
        Task<Response<bool>> CreateProduct(int userId, CreateProductModel model);
        Task<Response<bool>> UpdateProduct(int brandId, int userId, UpdateProductModel model);
        Task<Response<bool>> DeleteProduct(int brandId, int userId);
        Task<Response<ViewProductModel>> GetProductById(int id);
        Task<Response<List<ViewBatchModel>>> GetBatchesByProductId(SearchBatchRequest request);
        Task<Response<List<ViewProductModel>>> GetAllProduct();
        Task<Response<List<ViewProductModel>>> GetAllProduct(FilterProduct request);
        //Task<bool> CheckProduct(int brandId);
        Task<Response<List<ViewATS>>> GetListActiveSubstances(int productId);
        Task<Response<List<ViewModel>>> GetListRouteOfAdmin();
        Task<PageResult<ViewProductModel>> GetProductFilter(ProductFilterRequest request);
        //Task<PageResult<PageResult<ViewProductModel>>> GetProductPaging(ProductPagingRequest request);
    }
}

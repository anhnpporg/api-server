using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.BrandModel;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.BrandService
{
    public interface IBrandSvc
    {
        Task<Response<bool>> CreateBrand(int userId, CreateBrandModel model);
        Task<Response<bool>> UpdateBrand(int brandId, int userId, UpdateBrandModel model);
        Task<Response<bool>> DeleteBrand(int brandId, int userId);
        Task<Response<ViewBrandModel>> GetBrandById(int brandId);
        Task<Response<List<ViewBrandModel>>> GetAllBrand();
        //Task<bool> CheckBrand(int brandId);
        Task<Response<List<ViewProductModel>>> GetListProduct(int brandId);
    }
}

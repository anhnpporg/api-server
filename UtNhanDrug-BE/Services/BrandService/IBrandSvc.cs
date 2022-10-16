using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.BrandModel;
using UtNhanDrug_BE.Models.ProductModel;

namespace UtNhanDrug_BE.Services.BrandService
{
    public interface IBrandSvc
    {
        Task<bool> CreateBrand(int userId, CreateBrandModel model);
        Task<bool> UpdateBrand(int brandId, int userId, UpdateBrandModel model);
        Task<bool> DeleteBrand(int brandId, int userId);
        Task<ViewBrandModel> GetBrandById(int brandId);
        Task<List<ViewBrandModel>> GetAllBrand();
        Task<bool> CheckBrand(int brandId);
        Task<List<ViewProductModel>> GetListProduct(int brandId);
    }
}

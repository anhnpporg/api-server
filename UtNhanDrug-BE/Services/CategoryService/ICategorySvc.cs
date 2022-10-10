using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.CategoryModel;

namespace UtNhanDrug_BE.Services.CategoryService
{
    public interface ICategorySvc
    {
        Task<bool> CreateCategory(int userId, CreateCategoryModel model);
        Task<bool> UpdateCategory(int brandId, int userId, UpdateCategoryModel model);
        Task<bool> DeleteCategory(int brandId, int userId);
        Task<ViewCategoryModel> GetCategoryById(int brandId);
        Task<List<ViewCategoryModel>> GetAllCategory();
        Task<bool> CheckCategory(int brandId);
    }
}

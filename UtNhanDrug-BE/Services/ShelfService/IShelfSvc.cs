using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ShelfModel;
using UtNhanDrug_BE.Models.ProductModel;

namespace UtNhanDrug_BE.Services.ShelfService
{
    public interface IShelfSvc
    {
        Task<bool> CreateShelf(int userId, CreateShelfModel model);
        Task<bool> UpdateShelf(int brandId, int userId, UpdateShelfModel model);
        Task<bool> DeleteShelf(int brandId, int userId);
        Task<ViewShelfModel> GetShelfById(int shelfId);
        Task<List<ViewShelfModel>> GetAllShelves();
        Task<bool> CheckShelf(int brandId);
        Task<List<ViewProductModel>> GetListProduct(int shelfId);
    }
}

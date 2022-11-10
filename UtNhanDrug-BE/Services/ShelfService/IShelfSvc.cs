using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ShelfModel;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.ShelfService
{
    public interface IShelfSvc
    {
        Task<Response<bool>> CreateShelf(int userId, CreateShelfModel model);
        Task<Response<bool>> UpdateShelf(int brandId, int userId, UpdateShelfModel model);
        Task<Response<bool>> DeleteShelf(int brandId, int userId);
        Task<Response<ViewShelfModel>> GetShelfById(int shelfId);
        Task<Response<List<ViewShelfModel>>> GetAllShelves();
        //Task<bool> CheckShelf(int brandId);
        Task<Response<List<ViewProductModel>>> GetListProduct(int shelfId);
    }
}

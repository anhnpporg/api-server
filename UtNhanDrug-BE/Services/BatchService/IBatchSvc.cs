using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.BatchModel;

namespace UtNhanDrug_BE.Services.BatchService
{
    public interface IBatchSvc
    {
        Task<bool> CreateBatch(int userId, CreateBatchModel model);
        Task<bool> UpdateBatch(int id, int userId, UpdateBatchModel model);
        Task<bool> DeleteBatch(int id, int userId);
        Task<ViewBatchModel> GetBatchById(int id);
        Task<List<ViewBatchModel>> GetAllBatch();
        Task<bool> CheckBatch(int id);
    }
}

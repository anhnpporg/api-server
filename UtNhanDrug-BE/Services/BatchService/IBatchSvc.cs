using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.BatchModel;
using UtNhanDrug_BE.Models.GoodsReceiptNoteModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.BatchService
{
    public interface IBatchSvc
    {
        Task<bool> CreateBatch(int userId, CreateBatchModel model);
        Task<bool> UpdateBatch(int id, int userId, UpdateBatchModel model);
        Task<bool> DeleteBatch(int id, int userId);
        Task<List<ViewBatchModel>> GetBatchesByProductId(int id);
        Task<Response<List<ViewGoodsReceiptNoteModel>>> GetGRNByBatchId(int id);
        Task<Response<ViewBatchModel>> GetBatchesByBarcode(string barcode);
        Task<ViewBatchModel> GetBatchById(int id);
        Task<List<ViewBatchModel>> GetAllBatch();
        Task<bool> CheckBatch(int id);
        Task<Response<List<ViewQuantityInventoryModel>>> GetInventoryByUnitId(int unitId);
    }
}

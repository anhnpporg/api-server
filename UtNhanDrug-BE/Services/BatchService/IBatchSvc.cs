using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.BatchModel;
using UtNhanDrug_BE.Models.GoodsReceiptNoteModel;
using UtNhanDrug_BE.Models.InvoiceModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.BatchService
{
    public interface IBatchSvc
    {
        Task<Response<bool>> CreateBatch(int userId, CreateBatchModel model);
        Task<Response<bool>> UpdateBatch(int id, int userId, UpdateBatchModel model);
        Task<Response<bool>> DeleteBatch(int id, int userId);
        Task<Response<List<ViewGoodsReceiptNoteModel>>> GetGRNByBatchId(int id);
        Task<Response<List<ViewOrderDetailModel>>> GetGINByBatchId(int id);
        Task<Response<ViewBatchModel>> GetBatchesByBarcode(string barcode);
        Task<Response<ViewBatchModel>> GetBatchById(int id);
        Task<Response<List<ViewBatchModel>>> GetAllBatch();
        //Task<bool> CheckBatch(int id);
        Task<Response<List<ViewQuantityInventoryModel>>> GetInventoryByUnitId(int unitId);
    }
}

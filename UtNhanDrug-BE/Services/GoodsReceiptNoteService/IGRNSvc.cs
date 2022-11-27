using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.GoodsReceiptNoteModel;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.GoodsReceiptNoteService
{
    public interface IGRNSvc
    {
        Task<Response<bool>> CreateGoodsReceiptNote(int userId, CreateGoodsReceiptNoteModel model);
        Task<Response<bool>> UpdateGoodsReceiptNote(int id, int userId, UpdateGoodsReceiptNoteModel model);
        //Task<bool> DeleteConsignment(int id, int userId);
        Task<Response<ViewGoodsReceiptNoteModel>> GetGoodsReceiptNoteById(int id);
        Task<Response<List<ViewGoodsReceiptNoteModel>>> GetAllGoodsReceiptNote();
        Task<Response<List<ViewGoodsReceiptNoteModel>>> GetGoodsReceiptNoteByType(int type);
        //Task<bool> CheckGoodsReceiptNote(int id);
        //Task<Response<List<NoteLog>>> GetListNoteLog(int id);
        Task<Response<List<ViewModel>>> GetListNoteTypes();
    }
}

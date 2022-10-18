using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.GoodsReceiptNoteModel;

namespace UtNhanDrug_BE.Services.GoodsReceiptNoteService
{
    public interface IGRNSvc
    {
        Task<bool> CreateGoodsReceiptNote(int userId, CreateGoodsReceiptNoteModel model);
        Task<bool> UpdateGoodsReceiptNote(int id, int userId, UpdateGoodsReceiptNoteModel model);
        //Task<bool> DeleteConsignment(int id, int userId);
        Task<ViewGoodsReceiptNoteModel> GetGoodsReceiptNoteById(int id);
        Task<List<ViewGoodsReceiptNoteModel>> GetAllGoodsReceiptNote();
        Task<bool> CheckGoodsReceiptNote(int id);
        Task<List<NoteLog>> GetListNoteLog(int id);
    }
}

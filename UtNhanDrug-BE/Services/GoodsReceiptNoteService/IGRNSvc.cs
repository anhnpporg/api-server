﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.GoodsReceiptNoteModel;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.GoodsReceiptNoteService
{
    public interface IGRNSvc
    {
        Task<Response<bool>> CreateGoodsReceiptNote(int userId, List<CreateGoodsReceiptNoteModel> model);
        Task<bool> UpdateGoodsReceiptNote(int id, int userId, UpdateGoodsReceiptNoteModel model);
        //Task<bool> DeleteConsignment(int id, int userId);
        Task<ViewGoodsReceiptNoteModel> GetGoodsReceiptNoteById(int id);
        Task<List<ViewGoodsReceiptNoteModel>> GetAllGoodsReceiptNote();
        Task<bool> CheckGoodsReceiptNote(int id);
        Task<List<NoteLog>> GetListNoteLog(int id);
        Task<List<ViewModel>> GetListNoteTypes();
    }
}

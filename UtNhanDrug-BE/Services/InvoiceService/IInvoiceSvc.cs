﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.InvoiceModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.InvoiceService
{
    public interface IInvoiceSvc
    {
        Task<Response<InvoiceResponse>> CreateInvoice(int UserId, CreateInvoiceModel model);
        Task<Response<ViewInvoiceModel>> ViewInvoiceById(int Id);
        Task<Response<List<ViewInvoiceModel>>> GetAllInvoice();
        Task<Response<List<ViewInvoiceModel>>> GetInvoiceByUserId(int userId);
        Task<Response<List<ViewInvoiceModel>>> GetInvoiceCustomerId(int customerId);
        Task<Response<List<ViewOrderDetailModel>>> ViewOrderDetailByInvoiceId(int id);
        Task<Response<List<ViewOrderDetailModel>>> ViewOrderDetailByBarcode(string barcode);
        Task<Response<ViewInvoiceModel>> GetInvoiceByInvoiceBarcode(string invoiceBarcode);
        Task<Response<List<ViewGoodsIssueNoteModel>>> GetViewGoodsIssueNoteModel(List<int> GINId);
    }
}

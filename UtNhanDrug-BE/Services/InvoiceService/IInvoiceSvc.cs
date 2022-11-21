using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.InvoiceModel;
using UtNhanDrug_BE.Models.ResponseModel;

namespace UtNhanDrug_BE.Services.InvoiceService
{
    public interface IInvoiceSvc
    {
        Task<Response<bool>> CreateInvoice(int UserId, CreateInvoiceModel model);
        Task<Response<ViewInvoiceModel>> ViewInvoiceById(int Id);
        Task<Response<List<ViewInvoiceModel>>> GetAllInvoice();
        Task<Response<List<ViewInvoiceModel>>> GetInvoiceByUserId(int userId);
        Task<Response<List<ViewInvoiceModel>>> GetInvoiceCustomerId(int customerId);
        Task<Response<List<ViewOrderDetailModel>>> ViewOrderDetailByInvoiceId(int id);
        Task<Response<ViewInvoiceModel>> GetInvoiceByInvoiceBarcode(string invoiceBarcode);
    }
}

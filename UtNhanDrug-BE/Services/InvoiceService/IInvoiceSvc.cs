using System.Threading.Tasks;
using UtNhanDrug_BE.Models.InvoiceModel;

namespace UtNhanDrug_BE.Services.InvoiceService
{
    public interface IInvoiceSvc
    {
        Task<bool> CreateInvoice(int UserId, CreateInvoiceModel model);
    }
}

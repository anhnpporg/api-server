using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtNhanDrug_BE.Models.CustomerModel;

namespace UtNhanDrug_BE.Models.InvoiceModel
{
    public class CreateInvoiceModel
    {
        public int GoodsIssueNoteTypeId { get; set; }
        public int? CustomerId { get; set; }
        public decimal? BodyWeight { get; set; }
        public int? DayUse { get; set; }
        [Required]
        public List<OrderDetailModel> Product { get; set; }

        public CreateCustomerModel Customer { get; set; }
    }

    public class OrderDetailModel
    {
        public int ProductId { get; set; }
        public double? Dose { get; set; }
        public int? UnitDose { get; set; }
        public int? Frequency { get; set; }
        public int? DayUse { get; set; }
        public string Use { get; set; }

        public List<GoodsIssueNoteModel> GoodsIssueNote { get; set; }
    }

    public class GoodsIssueNoteModel
    {
        public int Quantity { get; set; }
        public int Unit { get; set; }
        public int BatchId { get; set; }
    }
}

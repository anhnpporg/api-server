using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtNhanDrug_BE.Models.CustomerModel;

namespace UtNhanDrug_BE.Models.InvoiceModel
{
    public class CreateInvoiceModel
    {
        public int? CustomerId { get; set; }
        [Required]
        public decimal? BodyWeight { get; set; }
        [Required]
        public int? DayUse { get; set; }
        public decimal Discount { get; set; }
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

        public GoodsIssueNoteModel GoodsIssueNote { get; set; }
    }

    public class GoodsIssueNoteModel
    {
        public int GoodsIssueNoteTypeId { get; set; }
        public int Quantity { get; set; }
        public int Unit { get; set; }
    }
}

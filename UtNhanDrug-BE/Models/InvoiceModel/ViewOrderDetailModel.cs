using System;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Models.InvoiceModel
{
    public class ViewOrderDetailModel
    {
        public int Id { get; set; }
        public int? OrderDetailId { get; set; }
        public ViewModel Product { get; set; }
        public double? Dose { get; set; }
        public int? UnitDose { get; set; }
        public int? Frequency { get; set; }
        public int? DayUse { get; set; }
        public string Use { get; set; }
        public ViewModel GoodsIssueNoteType { get; set; }
        public ViewModel Batch { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public int ConvertedQuantity { get; set; }
        public int ReturnedQuantity { get; set; }
        public ViewBaseProductUnit? ViewBaseProductUnit { get; set; }
        public decimal TotalPrice { get; set; }
        //public DateTime CreatTime { get; set; }
    }
    public class ViewBaseProductUnit
    {
        public string? BaseUnit { get; set; }
        public decimal? BaseUnitPrice { get; set; }
    }
}

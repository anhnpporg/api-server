using System;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Models.InvoiceModel
{
    public class ViewInvoiceModel
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public ViewCustomer? Customer { get; set; }
        public decimal? BodyWeight { get; set; }
        public int? DayUse { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public int IsReturn { get; set; }
        public DateTime CreatedAt { get; set; }
        public ViewModel CreatedBy { get; set; }
    }
    public class ViewCustomer
    {
        public int? Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
    }
}

namespace UtNhanDrug_BE.Models.InvoiceModel
{
    public class ViewGoodsIssueNoteModel
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public int ConvertedQuantity { get; set; }
    }
}

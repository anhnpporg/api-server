namespace UtNhanDrug_BE.Models.GoodsReceiptNoteModel
{
    public class UpdateGoodsReceiptNoteModel
    {
        public int GoodsReceiptNoteTypeId { get; set; }
        public int BatchId { get; set; }
        public int? Invoice { get; set; }
        public int? SupplierId { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal TotalPrice { get; set; }
        public int ConvertedQuantity { get; set; }
        public decimal BaseUnitPrice { get; set; }
        public string Note { get; set; }
    }
}

namespace UtNhanDrug_BE.Models.GoodsReceiptNoteModel
{
    public class UpdateGoodsReceiptNoteModel
    {
        public int GoodsReceiptNoteTypeId { get; set; }
        public int ConsignmentId { get; set; }
        public int? SupplierId { get; set; }
        public int Quantity { get; set; }
        public int UnitId { get; set; }
        public decimal PurchasePrice { get; set; }
        public int ConvertedQuantity { get; set; }
        public decimal? PurchasePriceBaseUnit { get; set; }
        public string Note { get; set; }
    }
}

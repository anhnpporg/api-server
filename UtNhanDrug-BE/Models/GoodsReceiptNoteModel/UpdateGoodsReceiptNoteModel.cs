using System.ComponentModel.DataAnnotations;

namespace UtNhanDrug_BE.Models.GoodsReceiptNoteModel
{
    public class UpdateGoodsReceiptNoteModel
    {
        public int GoodsReceiptNoteTypeId { get; set; }
        public int BatchId { get; set; }
        public int? SupplierId { get; set; }
        public int Quantity { get; set; }
        public int ProductUnitPriceId { get; set; }
        public decimal TotalPrice { get; set; }
        [Required]
        public string Note { get; set; }
    }
}

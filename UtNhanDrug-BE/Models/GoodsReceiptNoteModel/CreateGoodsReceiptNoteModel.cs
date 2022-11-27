using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtNhanDrug_BE.Models.BatchModel;
using UtNhanDrug_BE.Models.SupplierModel;

namespace UtNhanDrug_BE.Models.GoodsReceiptNoteModel
{
    public class CreateGoodsReceiptNoteModel
    {
        [Required]
        public int GoodsReceiptNoteTypeId { get; set; }
        public List<GoodsReceiptNoteCreateModel>? CreateModel { get; set; }
        public int? InvoiceId { get; set; }
        public bool IsFull { get; set; }
    }

    public class GoodsReceiptNoteCreateModel
    {
        public int? SupplierId { get; set; }
        public List<BatchesModel> Batches { get; set; }
        public CreateSupplierModel? Supplier { get; set; }
    }
    public class BatchesModel
    {
        public int? BatchId { get; set; }
        public int? Quantity { get; set; }
        public int? ProductUnitPriceId { get; set; }
        public decimal? TotalPrice { get; set; }
        public CreateBatchModel? Batch { get; set; }
    }
}

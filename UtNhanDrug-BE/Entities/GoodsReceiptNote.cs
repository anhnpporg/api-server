using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class GoodsReceiptNote
    {
        public GoodsReceiptNote()
        {
            GoodsReceiptNoteLogs = new HashSet<GoodsReceiptNoteLog>();
        }

        public int Id { get; set; }
        public int GoodsReceiptNoteTypeId { get; set; }
        public int BatchId { get; set; }
        public int? InvoiceId { get; set; }
        public int? SupplierId { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal TotalPrice { get; set; }
        public int ConvertedQuantity { get; set; }
        public decimal BaseUnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }

        public virtual Batch Batch { get; set; }
        public virtual UserAccount CreatedByNavigation { get; set; }
        public virtual GoodsReceiptNoteType GoodsReceiptNoteType { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<GoodsReceiptNoteLog> GoodsReceiptNoteLogs { get; set; }
    }
}

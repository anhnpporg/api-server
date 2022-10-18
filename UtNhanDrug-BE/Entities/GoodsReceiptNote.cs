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
        public int ConsignmentId { get; set; }
        public int? SupplierId { get; set; }
        public int Quantity { get; set; }
        public int UnitId { get; set; }
        public decimal PurchasePrice { get; set; }
        public int ConvertedQuantity { get; set; }
        public decimal? PurchasePriceBaseUnit { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }

        public virtual Consignment Consignment { get; set; }
        public virtual Staff CreatedByNavigation { get; set; }
        public virtual GoodsReceiptNoteType GoodsReceiptNoteType { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual ICollection<GoodsReceiptNoteLog> GoodsReceiptNoteLogs { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class GoodsIssueNote
    {
        public int Id { get; set; }
        public int GoodsIssueNoteTypeId { get; set; }
        public int? OrderDetailId { get; set; }
        public int BatchId { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public int ConvertedQuantity { get; set; }

        public virtual Batch Batch { get; set; }
        public virtual GoodsIssueNoteType GoodsIssueNoteType { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
    }
}

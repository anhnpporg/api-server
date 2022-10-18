using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class GoodsReceiptNoteLog
    {
        public int Id { get; set; }
        public int GoodsReceiptNoteId { get; set; }
        public string Note { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual GoodsReceiptNote GoodsReceiptNote { get; set; }
        public virtual Manager UpdatedByNavigation { get; set; }
    }
}

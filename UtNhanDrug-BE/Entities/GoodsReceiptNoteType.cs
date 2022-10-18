using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class GoodsReceiptNoteType
    {
        public GoodsReceiptNoteType()
        {
            GoodsReceiptNotes = new HashSet<GoodsReceiptNote>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<GoodsReceiptNote> GoodsReceiptNotes { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Supplier
    {
        public Supplier()
        {
            GoodsReceiptNotes = new HashSet<GoodsReceiptNote>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Manager CreatedByNavigation { get; set; }
        public virtual Manager UpdatedByNavigation { get; set; }
        public virtual ICollection<GoodsReceiptNote> GoodsReceiptNotes { get; set; }
    }
}

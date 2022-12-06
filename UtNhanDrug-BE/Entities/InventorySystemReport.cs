using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class InventorySystemReport
    {
        public int Id { get; set; }
        public int? BatchId { get; set; }
        public int? ProductId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Batch Batch { get; set; }
        public virtual Product Product { get; set; }
    }
}

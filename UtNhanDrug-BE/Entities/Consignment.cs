using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Consignment
    {
        public Consignment()
        {
            GoodsReceiptNotes = new HashSet<GoodsReceiptNote>();
            InventorySystemReports = new HashSet<InventorySystemReport>();
        }

        public int Id { get; set; }
        public string Barcode { get; set; }
        public int ProductId { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Staff CreatedByNavigation { get; set; }
        public virtual Product Product { get; set; }
        public virtual Manager UpdatedByNavigation { get; set; }
        public virtual ICollection<GoodsReceiptNote> GoodsReceiptNotes { get; set; }
        public virtual ICollection<InventorySystemReport> InventorySystemReports { get; set; }
    }
}

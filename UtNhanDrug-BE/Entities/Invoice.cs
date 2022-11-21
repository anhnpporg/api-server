using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Invoice
    {
        public Invoice()
        {
            CustomerPointTransactions = new HashSet<CustomerPointTransaction>();
            GoodsReceiptNotes = new HashSet<GoodsReceiptNote>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string Barcode { get; set; }
        public decimal? BodyWeight { get; set; }
        public int? DayUse { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }

        public virtual UserAccount CreatedByNavigation { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<CustomerPointTransaction> CustomerPointTransactions { get; set; }
        public virtual ICollection<GoodsReceiptNote> GoodsReceiptNotes { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

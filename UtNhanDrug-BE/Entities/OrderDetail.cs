using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            GoodsIssueNotes = new HashSet<GoodsIssueNote>();
        }

        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public double? Dose { get; set; }
        public int? UnitDose { get; set; }
        public int? Frequency { get; set; }
        public int? DayUse { get; set; }
        public string Use { get; set; }
        public decimal TotalPrice { get; set; }

        public virtual Invoice Invoice { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<GoodsIssueNote> GoodsIssueNotes { get; set; }
    }
}

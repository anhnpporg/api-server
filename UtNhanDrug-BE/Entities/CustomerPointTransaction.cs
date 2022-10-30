using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class CustomerPointTransaction
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int InvoiceId { get; set; }
        public double Point { get; set; }
        public bool? IsReciept { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Customer
    {
        public Customer()
        {
            CustomerPointTransactions = new HashSet<CustomerPointTransaction>();
            Invoices = new HashSet<Invoice>();
        }

        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public double TotalPoint { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual UserAccount CreatedByNavigation { get; set; }
        public virtual UserAccount UpdatedByNavigation { get; set; }
        public virtual ICollection<CustomerPointTransaction> CustomerPointTransactions { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}

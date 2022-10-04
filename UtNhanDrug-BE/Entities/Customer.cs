using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Customer
    {
        public int Id { get; set; }
        public int UserAccountId { get; set; }
        public string PhoneNumber { get; set; }

        public virtual UserAccount UserAccount { get; set; }
    }
}

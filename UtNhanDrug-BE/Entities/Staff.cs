using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Staff
    {
        public int Id { get; set; }
        public int UserAccountId { get; set; }
        public string UrlAvartar { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsMale { get; set; }

        public virtual UserAccount UserAccount { get; set; }
    }
}

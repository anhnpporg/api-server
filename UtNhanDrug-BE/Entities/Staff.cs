using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Staff
    {
        public Staff()
        {
            Consignments = new HashSet<Consignment>();
            GoodsReceiptNotes = new HashSet<GoodsReceiptNote>();
        }

        public int Id { get; set; }
        public int UserAccountId { get; set; }
        public string Avatar { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsMale { get; set; }

        public virtual UserAccount UserAccount { get; set; }
        public virtual ICollection<Consignment> Consignments { get; set; }
        public virtual ICollection<GoodsReceiptNote> GoodsReceiptNotes { get; set; }
    }
}

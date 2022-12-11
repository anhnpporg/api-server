using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Manager
    {
        public int Id { get; set; }
        public string FcmToken { get; set; }
        public int UserAccountId { get; set; }

        public virtual UserAccount UserAccount { get; set; }
    }
}

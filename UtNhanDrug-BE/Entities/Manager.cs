using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Manager
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }

        public virtual User User { get; set; }
    }
}

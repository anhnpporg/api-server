using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class UserAccount
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Manager Manager { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual UserLoginDataExternal UserLoginDataExternal { get; set; }
        public virtual UserLoginDatum UserLoginDatum { get; set; }
    }
}

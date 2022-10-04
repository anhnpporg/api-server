using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class EmailValidationStatus
    {
        public EmailValidationStatus()
        {
            UserLoginData = new HashSet<UserLoginDatum>();
        }

        public int Id { get; set; }
        public string StatusDescription { get; set; }

        public virtual ICollection<UserLoginDatum> UserLoginData { get; set; }
    }
}

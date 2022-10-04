using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class UserLoginDataExternal
    {
        public int Id { get; set; }
        public int UserAccountId { get; set; }
        public int ExternalProviderId { get; set; }
        public string ExternalProviderToken { get; set; }

        public virtual ExternalProvider UserAccount { get; set; }
        public virtual UserAccount UserAccountNavigation { get; set; }
    }
}

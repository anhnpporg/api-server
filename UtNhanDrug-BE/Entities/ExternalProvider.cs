using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class ExternalProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WebServiceEndPoint { get; set; }

        public virtual UserLoginDataExternal UserLoginDataExternal { get; set; }
    }
}

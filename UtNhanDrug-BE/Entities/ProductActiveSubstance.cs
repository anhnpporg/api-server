using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class ProductActiveSubstance
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ActiveSubstanceId { get; set; }

        public virtual ActiveSubstance ActiveSubstance { get; set; }
        public virtual Product Product { get; set; }
    }
}

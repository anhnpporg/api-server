using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class ProductUnit
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public decimal? ConversionValue { get; set; }
        public decimal? Price { get; set; }
        public bool IsBaseUnit { get; set; }

        public virtual Product Product { get; set; }
        public virtual Unit Unit { get; set; }
    }
}

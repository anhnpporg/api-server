using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Unit
    {
        public Unit()
        {
            GoodsReceiptNotes = new HashSet<GoodsReceiptNote>();
            ProductUnits = new HashSet<ProductUnit>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<GoodsReceiptNote> GoodsReceiptNotes { get; set; }
        public virtual ICollection<ProductUnit> ProductUnits { get; set; }
    }
}

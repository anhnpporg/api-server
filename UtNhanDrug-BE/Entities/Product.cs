using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Product
    {
        public Product()
        {
            ProductActiveSubstances = new HashSet<ProductActiveSubstance>();
        }

        public int Id { get; set; }
        public string DrugRegistrationNumber { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int MinimumQuantity { get; set; }
        public decimal? Dosage { get; set; }
        public int? DosageUnitId { get; set; }
        public int? UnitId { get; set; }
        public decimal Price { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        public virtual Manager CreatedByNavigation { get; set; }
        public virtual DosageUnit DosageUnit { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual Manager UpdatedByNavigation { get; set; }
        public virtual ICollection<ProductActiveSubstance> ProductActiveSubstances { get; set; }
    }
}

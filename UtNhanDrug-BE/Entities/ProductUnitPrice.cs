using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class ProductUnitPrice
    {
        public ProductUnitPrice()
        {
            SamplePrescriptionDetails = new HashSet<SamplePrescriptionDetail>();
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Unit { get; set; }
        public double? ConversionValue { get; set; }
        public decimal? Price { get; set; }
        public bool IsPackingSpecification { get; set; }
        public bool IsDoseBasedOnBodyWeightUnit { get; set; }
        public bool IsBaseUnit { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual UserAccount CreatedByNavigation { get; set; }
        public virtual Product Product { get; set; }
        public virtual UserAccount UpdatedByNavigation { get; set; }
        public virtual ICollection<SamplePrescriptionDetail> SamplePrescriptionDetails { get; set; }
    }
}

using System;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Models.ProductUnitModel
{
    public class ViewProductUnitPriceModel
    {
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
    }
}

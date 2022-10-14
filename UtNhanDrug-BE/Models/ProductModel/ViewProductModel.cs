using System;
using System.Collections.Generic;
using UtNhanDrug_BE.Models.ProductActiveSubstance;

namespace UtNhanDrug_BE.Models.ProductModel
{
    public class ViewProductModel
    {
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
        public List<ViewPASModel> ProductActiveSubstance { get; set; }
        public decimal Price { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}

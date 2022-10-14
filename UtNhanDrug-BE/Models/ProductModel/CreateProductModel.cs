using System;
using System.Collections.Generic;
using UtNhanDrug_BE.Models.ProductActiveSubstance;

namespace UtNhanDrug_BE.Models.ProductModel
{
    public class CreateProductModel
    {
        public string DrugRegistrationNumber { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int MinimumQuantity { get; set; }
        public decimal? Dosage { get; set; }
        public int? DosageUnitId { get; set; }
        public int? UnitId { get; set; }
        public decimal Price { get; set; }
        public List<int> ActiveSubstances { get; set; }
    }
}

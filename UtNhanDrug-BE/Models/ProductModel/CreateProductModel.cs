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
        public int ShelfId { get; set; }
        public int MinimumQuantity { get; set; }
        public decimal? StockStrength { get; set; }
        public int? StockStrengthUnitId { get; set; }
        public int? RouteOfAdministrationId { get; set; }
        public bool IsMedicine { get; set; }
        public bool IsConsignment { get; set; }
        public List<int> ActiveSubstances { get; set; }
    }
}

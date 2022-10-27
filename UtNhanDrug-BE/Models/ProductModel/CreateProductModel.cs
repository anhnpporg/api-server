using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtNhanDrug_BE.Models.ProductUnitModel;

namespace UtNhanDrug_BE.Models.ProductModel
{
    public class CreateProductModel
    {
        [Required]
        public string DrugRegistrationNumber { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int BrandId { get; set; }
        [Required]
        public int ShelfId { get; set; }
        [Required]
        public int MinimumQuantity { get; set; }
        public decimal? StockStrength { get; set; }
        public int? StockStrengthUnitId { get; set; }
        public int? RouteOfAdministrationId { get; set; }
        [Required]
        public bool IsMedicine { get; set; }
        [Required]
        public bool IsConsignment { get; set; }
        [Required]
        public List<int> ActiveSubstances { get; set; }

        // create base unit
        [Required]
        public int UnitId { get; set; }
        [Required]
        public decimal? Price { get; set; }

        //create product unit (optional)
        public List<ProductUnitModels> ProductUnits { get; set; }
    }

}

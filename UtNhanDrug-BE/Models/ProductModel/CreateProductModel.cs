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
        public int MininumInventory { get; set; }
        public int RouteOfAdministrationId { get; set; }
        [Required]
        public bool IsUseDose { get; set; }
        [Required]
        public bool IsManagedInBatches { get; set; }
        [Required]
        public List<int> ActiveSubstances { get; set; }

        // create base unit
        public string Unit { get; set; }
        public decimal? Price { get; set; }
        public bool IsPackingSpecification { get; set; }
        public bool IsDoseBasedOnBodyWeightUnit { get; set; }

        //create product unit (optional)
        public List<ProductUnitPriceModels> ProductUnits { get; set; }
    }

}

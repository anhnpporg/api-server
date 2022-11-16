﻿using System;
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
        public bool IsManagedInBatches { get; set; }
        [Required]
        public List<int> ActiveSubstances { get; set; }

        // create base unit
        public string Unit { get; set; }
        public decimal? Price { get; set; }

        //create product unit (optional)
        public List<ProductUnitPriceModels> ProductUnits { get; set; }
        //dose unit
        [Required]
        public bool IsUseDose { get; set; }
        public DoseUnitPrice DoseUnitPrice { get; set; }
    }
    public class DoseUnitPrice
    {
        public string DoseUnit { get; set; }
        public double ConversionValue { get; set; }
    }

}

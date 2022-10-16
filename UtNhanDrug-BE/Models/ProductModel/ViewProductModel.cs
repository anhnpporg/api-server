using System;
using System.Collections.Generic;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.BrandModel;
using UtNhanDrug_BE.Models.ProductActiveSubstance;

namespace UtNhanDrug_BE.Models.ProductModel
{
    public class ViewProductModel
    {
        public int Id { get; set; }
        public string DrugRegistrationNumber { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public ViewModel Brand { get; set; }
        public ViewModel Category { get; set; }
        public int MinimumQuantity { get; set; }
        public decimal? Dosage { get; set; }
        public ViewModel? DosageUnit { get; set; }
        public ViewModel? Unit { get; set; }
        public decimal Price { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public List<ViewModel> ActiveSubstances { get; set; }
    }

    public class ViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}

using System;
using System.Collections.Generic;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.ProductUnitModel;

namespace UtNhanDrug_BE.Models.ProductModel
{
    public class ViewProductModel
    {
        public int Id { get; set; }
        public string DrugRegistrationNumber { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public ViewModel Brand { get; set; }
        public ViewModel Shelf { get; set; }
        public ViewModel RouteOfAdministration { get; set; }
        public int MininumInventory { get; set; }
        public bool IsUseDose { get; set; }
        public bool IsManagedInBatches { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public List<ViewModel> ActiveSubstances { get; set; }
        public List<ViewProductUnitPriceModel> ProductUnits { get; set; }
    }


}

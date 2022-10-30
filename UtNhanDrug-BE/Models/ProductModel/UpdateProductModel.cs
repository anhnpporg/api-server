using System;

namespace UtNhanDrug_BE.Models.ProductModel
{
    public class UpdateProductModel
    {
        public string DrugRegistrationNumber { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public int ShelfId { get; set; }
        public int MininumInventory { get; set; }
        public int RouteOfAdministrationId { get; set; }
        public bool IsUseDose { get; set; }
        public bool IsManagedInBatches { get; set; }
    }
}

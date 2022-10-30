using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Product
    {
        public Product()
        {
            Batches = new HashSet<Batch>();
            OrderDetails = new HashSet<OrderDetail>();
            ProductActiveSubstances = new HashSet<ProductActiveSubstance>();
            ProductUnitPrices = new HashSet<ProductUnitPrice>();
            SamplePrescriptionDetails = new HashSet<SamplePrescriptionDetail>();
        }

        public int Id { get; set; }
        public string DrugRegistrationNumber { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public int ShelfId { get; set; }
        public int RouteOfAdministrationId { get; set; }
        public int MininumInventory { get; set; }
        public bool IsUseDose { get; set; }
        public bool IsManagedInBatches { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual UserAccount CreatedByNavigation { get; set; }
        public virtual RouteOfAdministration RouteOfAdministration { get; set; }
        public virtual Shelf Shelf { get; set; }
        public virtual UserAccount UpdatedByNavigation { get; set; }
        public virtual ICollection<Batch> Batches { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductActiveSubstance> ProductActiveSubstances { get; set; }
        public virtual ICollection<ProductUnitPrice> ProductUnitPrices { get; set; }
        public virtual ICollection<SamplePrescriptionDetail> SamplePrescriptionDetails { get; set; }
    }
}

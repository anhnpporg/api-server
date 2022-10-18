using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Product
    {
        public Product()
        {
            Consignments = new HashSet<Consignment>();
            ProductActiveSubstances = new HashSet<ProductActiveSubstance>();
            ProductUnits = new HashSet<ProductUnit>();
            SamplePrescriptionDetails = new HashSet<SamplePrescriptionDetail>();
        }

        public int Id { get; set; }
        public string DrugRegistrationNumber { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public int ShelfId { get; set; }
        public int MinimumQuantity { get; set; }
        public decimal? StockStrength { get; set; }
        public int? StockStrengthUnitId { get; set; }
        public int? RouteOfAdministrationId { get; set; }
        public bool IsMedicine { get; set; }
        public bool IsConsignment { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Manager CreatedByNavigation { get; set; }
        public virtual RouteOfAdministration RouteOfAdministration { get; set; }
        public virtual Shelf Shelf { get; set; }
        public virtual StockStrengthUnit StockStrengthUnit { get; set; }
        public virtual Manager UpdatedByNavigation { get; set; }
        public virtual ICollection<Consignment> Consignments { get; set; }
        public virtual ICollection<ProductActiveSubstance> ProductActiveSubstances { get; set; }
        public virtual ICollection<ProductUnit> ProductUnits { get; set; }
        public virtual ICollection<SamplePrescriptionDetail> SamplePrescriptionDetails { get; set; }
    }
}

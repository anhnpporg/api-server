using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Manager
    {
        public Manager()
        {
            ActiveSubstanceCreatedByNavigations = new HashSet<ActiveSubstance>();
            ActiveSubstanceUpdatedByNavigations = new HashSet<ActiveSubstance>();
            BrandCreatedByNavigations = new HashSet<Brand>();
            BrandUpdatedByNavigations = new HashSet<Brand>();
            Consignments = new HashSet<Consignment>();
            DiseaseCreatedByNavigations = new HashSet<Disease>();
            DiseaseUpdatedByNavigations = new HashSet<Disease>();
            GoodsReceiptNoteLogs = new HashSet<GoodsReceiptNoteLog>();
            InventorySystemReports = new HashSet<InventorySystemReport>();
            ProductCreatedByNavigations = new HashSet<Product>();
            ProductUpdatedByNavigations = new HashSet<Product>();
            SamplePrescriptionCreatedByNavigations = new HashSet<SamplePrescription>();
            SamplePrescriptionDetailCreatedByNavigations = new HashSet<SamplePrescriptionDetail>();
            SamplePrescriptionDetailUpdatedByNavigations = new HashSet<SamplePrescriptionDetail>();
            SamplePrescriptionUpdatedByNavigations = new HashSet<SamplePrescription>();
            ShelfCreatedByNavigations = new HashSet<Shelf>();
            ShelfUpdatedByNavigations = new HashSet<Shelf>();
            SupplierCreatedByNavigations = new HashSet<Supplier>();
            SupplierUpdatedByNavigations = new HashSet<Supplier>();
        }

        public int Id { get; set; }
        public int UserAccountId { get; set; }

        public virtual UserAccount UserAccount { get; set; }
        public virtual ICollection<ActiveSubstance> ActiveSubstanceCreatedByNavigations { get; set; }
        public virtual ICollection<ActiveSubstance> ActiveSubstanceUpdatedByNavigations { get; set; }
        public virtual ICollection<Brand> BrandCreatedByNavigations { get; set; }
        public virtual ICollection<Brand> BrandUpdatedByNavigations { get; set; }
        public virtual ICollection<Consignment> Consignments { get; set; }
        public virtual ICollection<Disease> DiseaseCreatedByNavigations { get; set; }
        public virtual ICollection<Disease> DiseaseUpdatedByNavigations { get; set; }
        public virtual ICollection<GoodsReceiptNoteLog> GoodsReceiptNoteLogs { get; set; }
        public virtual ICollection<InventorySystemReport> InventorySystemReports { get; set; }
        public virtual ICollection<Product> ProductCreatedByNavigations { get; set; }
        public virtual ICollection<Product> ProductUpdatedByNavigations { get; set; }
        public virtual ICollection<SamplePrescription> SamplePrescriptionCreatedByNavigations { get; set; }
        public virtual ICollection<SamplePrescriptionDetail> SamplePrescriptionDetailCreatedByNavigations { get; set; }
        public virtual ICollection<SamplePrescriptionDetail> SamplePrescriptionDetailUpdatedByNavigations { get; set; }
        public virtual ICollection<SamplePrescription> SamplePrescriptionUpdatedByNavigations { get; set; }
        public virtual ICollection<Shelf> ShelfCreatedByNavigations { get; set; }
        public virtual ICollection<Shelf> ShelfUpdatedByNavigations { get; set; }
        public virtual ICollection<Supplier> SupplierCreatedByNavigations { get; set; }
        public virtual ICollection<Supplier> SupplierUpdatedByNavigations { get; set; }
    }
}

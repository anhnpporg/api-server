using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class UserAccount
    {
        public UserAccount()
        {
            ActiveSubstanceCreatedByNavigations = new HashSet<ActiveSubstance>();
            ActiveSubstanceUpdatedByNavigations = new HashSet<ActiveSubstance>();
            BatchCreatedByNavigations = new HashSet<Batch>();
            BatchUpdatedByNavigations = new HashSet<Batch>();
            BrandCreatedByNavigations = new HashSet<Brand>();
            BrandUpdatedByNavigations = new HashSet<Brand>();
            CustomerCreatedByNavigations = new HashSet<Customer>();
            CustomerUpdatedByNavigations = new HashSet<Customer>();
            DiseaseCreatedByNavigations = new HashSet<Disease>();
            DiseaseDeletedByNavigations = new HashSet<Disease>();
            DiseaseUpdatedByNavigations = new HashSet<Disease>();
            GoodsReceiptNoteLogs = new HashSet<GoodsReceiptNoteLog>();
            GoodsReceiptNotes = new HashSet<GoodsReceiptNote>();
            Invoices = new HashSet<Invoice>();
            ProductCreatedByNavigations = new HashSet<Product>();
            ProductUnitPriceCreatedByNavigations = new HashSet<ProductUnitPrice>();
            ProductUnitPriceUpdatedByNavigations = new HashSet<ProductUnitPrice>();
            ProductUpdatedByNavigations = new HashSet<Product>();
            SamplePrescriptionCreatedByNavigations = new HashSet<SamplePrescription>();
            SamplePrescriptionDeletedByNavigations = new HashSet<SamplePrescription>();
            SamplePrescriptionDetailCreatedByNavigations = new HashSet<SamplePrescriptionDetail>();
            SamplePrescriptionDetailDeletedByNavigations = new HashSet<SamplePrescriptionDetail>();
            SamplePrescriptionDetailUpdatedByNavigations = new HashSet<SamplePrescriptionDetail>();
            SamplePrescriptionUpdatedByNavigations = new HashSet<SamplePrescription>();
            ShelfCreatedByNavigations = new HashSet<Shelf>();
            ShelfUpdatedByNavigations = new HashSet<Shelf>();
            SupplierCreatedByNavigations = new HashSet<Supplier>();
            SupplierUpdatedByNavigations = new HashSet<Supplier>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Manager Manager { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual UserLoginDatum UserLoginDatum { get; set; }
        public virtual ICollection<ActiveSubstance> ActiveSubstanceCreatedByNavigations { get; set; }
        public virtual ICollection<ActiveSubstance> ActiveSubstanceUpdatedByNavigations { get; set; }
        public virtual ICollection<Batch> BatchCreatedByNavigations { get; set; }
        public virtual ICollection<Batch> BatchUpdatedByNavigations { get; set; }
        public virtual ICollection<Brand> BrandCreatedByNavigations { get; set; }
        public virtual ICollection<Brand> BrandUpdatedByNavigations { get; set; }
        public virtual ICollection<Customer> CustomerCreatedByNavigations { get; set; }
        public virtual ICollection<Customer> CustomerUpdatedByNavigations { get; set; }
        public virtual ICollection<Disease> DiseaseCreatedByNavigations { get; set; }
        public virtual ICollection<Disease> DiseaseDeletedByNavigations { get; set; }
        public virtual ICollection<Disease> DiseaseUpdatedByNavigations { get; set; }
        public virtual ICollection<GoodsReceiptNoteLog> GoodsReceiptNoteLogs { get; set; }
        public virtual ICollection<GoodsReceiptNote> GoodsReceiptNotes { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Product> ProductCreatedByNavigations { get; set; }
        public virtual ICollection<ProductUnitPrice> ProductUnitPriceCreatedByNavigations { get; set; }
        public virtual ICollection<ProductUnitPrice> ProductUnitPriceUpdatedByNavigations { get; set; }
        public virtual ICollection<Product> ProductUpdatedByNavigations { get; set; }
        public virtual ICollection<SamplePrescription> SamplePrescriptionCreatedByNavigations { get; set; }
        public virtual ICollection<SamplePrescription> SamplePrescriptionDeletedByNavigations { get; set; }
        public virtual ICollection<SamplePrescriptionDetail> SamplePrescriptionDetailCreatedByNavigations { get; set; }
        public virtual ICollection<SamplePrescriptionDetail> SamplePrescriptionDetailDeletedByNavigations { get; set; }
        public virtual ICollection<SamplePrescriptionDetail> SamplePrescriptionDetailUpdatedByNavigations { get; set; }
        public virtual ICollection<SamplePrescription> SamplePrescriptionUpdatedByNavigations { get; set; }
        public virtual ICollection<Shelf> ShelfCreatedByNavigations { get; set; }
        public virtual ICollection<Shelf> ShelfUpdatedByNavigations { get; set; }
        public virtual ICollection<Supplier> SupplierCreatedByNavigations { get; set; }
        public virtual ICollection<Supplier> SupplierUpdatedByNavigations { get; set; }
    }
}

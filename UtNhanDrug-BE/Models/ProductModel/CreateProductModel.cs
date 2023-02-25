using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UtNhanDrug_BE.Models.BatchModel;
using UtNhanDrug_BE.Models.ProductUnitModel;
using UtNhanDrug_BE.Models.SupplierModel;

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
        public List<int>? ActiveSubstances { get; set; }

        // create base unit
        public string Unit { get; set; }
        public decimal? Price { get; set; }

        //create product unit (optional)
        public List<ProductUnitPriceModels> ProductUnits { get; set; }
        //dose unit
        [Required]
        public bool IsUseDose { get; set; }
        public DoseUnitPrice DoseUnitPrice { get; set; }
        //GRN model
        public List<GoodsReceiptNoteCreateModel>? CreateModel { get; set; }
    }
    public class DoseUnitPrice
    {
        public string DoseUnit { get; set; }
        public double ConversionValue { get; set; }
    }

    //GRN model

    public class GoodsReceiptNoteCreateModel
    {
        public int? SupplierId { get; set; }
        public List<BatchesModel> Batches { get; set; }
        public CreateSupplierModel? Supplier { get; set; }
    }
    public class BatchesModel
    {
        public int? BatchId { get; set; }
        public int? Quantity { get; set; }
        public int? ProductUnitPriceId { get; set; }
        public decimal? TotalPrice { get; set; }
        public CreateBatchModel? Batch { get; set; }
    }

}

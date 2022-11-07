using System;

namespace UtNhanDrug_BE.Models.BatchModel
{
    public class ViewQuantityInventoryModel
    {
        public int BatchId { get; set; }
        public string BatchBarcode { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Unit { get; set; }
        public int CurrentQuantity { get; set; }
    }
}

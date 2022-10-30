using System;

namespace UtNhanDrug_BE.Models.BatchModel
{
    public class CreateBatchModel
    {
        public string BatchBarcode { get; set; }
        public int ProductId { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int CreatedBy { get; set; }
    }
}

using System;

namespace UtNhanDrug_BE.Models.BatchModel
{
    public class UpdateBatchModel
    {
        public int ProductId { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}

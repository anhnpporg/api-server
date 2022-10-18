using System;

namespace UtNhanDrug_BE.Models.ConsignmentModel
{
    public class CreateConsignmentModel
    {
        public int ProductId { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? IsActive { get; set; }
    }
}

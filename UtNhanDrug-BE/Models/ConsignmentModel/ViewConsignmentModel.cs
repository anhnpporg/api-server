using System;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Models.ConsignmentModel
{
    public class ViewConsignmentModel
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public ViewModel Product { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public ViewModel CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ViewModel? UpdatedBy { get; set; }
    }
}

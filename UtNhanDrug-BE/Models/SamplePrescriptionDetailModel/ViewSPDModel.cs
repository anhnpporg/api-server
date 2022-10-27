using System;
using UtNhanDrug_BE.Models.ModelHelper;
using UtNhanDrug_BE.Models.ProductModel;

namespace UtNhanDrug_BE.Models.SamplePrescriptionDetailModel
{
    public class ViewSPDModel
    {
        public int Id { get; set; }
        public SPDModel SamplePrescription { get; set; }
        public ViewProductModel Product { get; set; }
        public int? Dose { get; set; }
        public decimal? DoseBasedOnBodyWeight { get; set; }
        public int? FrequencyPerDay { get; set; }
        public string Use { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public ViewModel CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ViewModel? UpdatedBy { get; set; }
    }
    public class SPDModel
    {
        public int Id { get; set; }
        public int SamplePrescriptionId { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}

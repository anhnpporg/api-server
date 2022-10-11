using System;

namespace UtNhanDrug_BE.Models.SamplePrescriptionModel
{
    public class ViewSamplePrescriptionModel
    {
        public int Id { get; set; }
        public int DiseaseId { get; set; }
        public decimal? CustomerWeight { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}

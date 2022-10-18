namespace UtNhanDrug_BE.Models.SamplePrescriptionModel
{
    public class UpdateSamplePrescriptionModel
    {
        public int DiseaseId { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}

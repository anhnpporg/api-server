namespace UtNhanDrug_BE.Models.SamplePrescriptionDetailModel
{
    public class UpdateSPDModel
    {
        public int ProductId { get; set; }
        public int? Dose { get; set; }
        public decimal? DoseBasedOnBodyWeight { get; set; }
        public int? FrequencyPerDay { get; set; }
        public string Use { get; set; }
    }
}

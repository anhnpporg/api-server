namespace UtNhanDrug_BE.Models.SamplePrescriptionDetailModel
{
    public class CreateSPDModel
    {
        public int SamplePrescriptionId { get; set; }
        public int ProductId { get; set; }
        public double Dose { get; set; }
        public int ProductUnitPriceId { get; set; }
        public int? Frequency { get; set; }
        public string Use { get; set; }
        public int CreatedBy { get; set; }
    }
}   

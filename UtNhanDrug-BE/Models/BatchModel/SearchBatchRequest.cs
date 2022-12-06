namespace UtNhanDrug_BE.Models.BatchModel
{
    public class SearchBatchRequest
    {
        public int ProductId { get; set; }
        public bool? IsSale { get; set; } = false;
    }
}

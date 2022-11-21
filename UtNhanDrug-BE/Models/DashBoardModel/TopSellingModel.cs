namespace UtNhanDrug_BE.Models.DashBoardModel
{
    public class TopSellingModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Sold { get; set; }
        public string Unit { get; set; }
        public decimal Revenue { get; set; }
    }
}

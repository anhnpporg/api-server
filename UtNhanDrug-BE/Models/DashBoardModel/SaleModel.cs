namespace UtNhanDrug_BE.Models.DashBoardModel
{
    public class SaleModel
    {
        public int QuantityOrder { get; set; }
        public double PercentQuantityOrder { get; set; }
        public decimal Cost { get; set; }
        public double PercentCost { get; set; }
        public decimal Turnover { get; set; }
        public double PercentTurnover { get; set; }
        public decimal Profit { get; set; }
        public double PercentProfit { get; set; }
    }
}

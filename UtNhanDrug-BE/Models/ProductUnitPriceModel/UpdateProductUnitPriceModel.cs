namespace UtNhanDrug_BE.Models.ProductUnitModel
{
    public class UpdateProductUnitPriceModel
    {
        public int ProductId { get; set; }
        public int ProductUnitId { get; set; }
        public string Unit { get; set; }
        public double? ConversionValue { get; set; }
        public decimal? Price { get; set; }
    }
}

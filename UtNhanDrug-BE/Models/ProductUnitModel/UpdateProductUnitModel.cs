namespace UtNhanDrug_BE.Models.ProductUnitModel
{
    public class UpdateProductUnitModel
    {
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public decimal? ConversionValue { get; set; }
        public decimal? Price { get; set; }
        public bool IsBaseUnit { get; set; }
    }
}

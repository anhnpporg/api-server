using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Models.ProductUnitModel
{
    public class ViewProductUnitModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ViewModel Unit { get; set; }
        public decimal? ConversionValue { get; set; }
        public decimal? Price { get; set; }
        public bool IsBaseUnit { get; set; }
    }
}

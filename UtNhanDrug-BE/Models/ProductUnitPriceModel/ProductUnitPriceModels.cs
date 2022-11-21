using Newtonsoft.Json;

namespace UtNhanDrug_BE.Models.ProductUnitModel
{

    public class ProductUnitPriceModels
    {
        public string Unit { get; set; }
        public double? ConversionValue { get; set; }
        public decimal? Price { get; set; }
    }
}

using Newtonsoft.Json;

namespace UtNhanDrug_BE.Models.ProductUnitModel
{

    public class ProductUnitPriceModels
    {
        public string Unit { get; set; }
        public double? ConversionValue { get; set; }
        public decimal? Price { get; set; }
        public bool IsPackingSpecification { get; set; }
        public bool IsDoseBasedOnBodyWeightUnit { get; set; }
        public int CreatedBy { get; set; }
    }
}

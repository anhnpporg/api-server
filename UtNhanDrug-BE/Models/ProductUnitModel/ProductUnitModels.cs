using Newtonsoft.Json;

namespace UtNhanDrug_BE.Models.ProductUnitModel
{

    public class ProductUnitModels
    {
        [JsonProperty("UnitId")]
        public int UnitId { get; set; }
        [JsonProperty("ConversionValue")]
        public decimal? ConversionValue { get; set; }
        [JsonProperty("Price")]
        public decimal? Price { get; set; }
    }
}

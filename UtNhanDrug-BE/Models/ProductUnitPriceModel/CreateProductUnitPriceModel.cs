﻿namespace UtNhanDrug_BE.Models.ProductUnitModel
{
    public class CreateProductUnitPriceModel
    {
        public int ProductId { get; set; }
        public string Unit { get; set; }
        public double? ConversionValue { get; set; }
        public decimal? Price { get; set; }
    }
}

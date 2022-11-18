namespace UtNhanDrug_BE.Models.ProductModel
{
    public class FilterProduct
    {
        public bool IsProductActive { get; set; }
        public bool IsSupplierActive { get; set; }
        public bool IsBrandActive { get; set; }
        public bool IsActiveActiveSubstance { get; set; }
    }
}

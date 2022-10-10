using System.Diagnostics.CodeAnalysis;

namespace UtNhanDrug_BE.Models.UnitModel
{
    public class CreateUnitModel
    {
        [NotNull]
        public string Name { get; set; }
    }
}

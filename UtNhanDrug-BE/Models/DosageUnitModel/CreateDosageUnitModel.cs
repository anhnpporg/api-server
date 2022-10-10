using System.Diagnostics.CodeAnalysis;

namespace UtNhanDrug_BE.Models.DosageUnitModel
{
    public class CreateDosageUnitModel
    {
        [NotNull]
        public string Name { get; set; }
    }
}

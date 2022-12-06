using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static UtNhanDrug_BE.Models.ProfileUserModel.ProfileUserViewModel;

namespace UtNhanDrug_BE.Models.SamplePrescriptionDetailModel
{
    public class SamplePrescriptionDetailViewModel
    {
        public class SamplePrescriptionDetailForManager
        {
            public int Id { get; set; }
            public int SamplePrescriptionId { get; set; }
            public int ProductId { get; set; }
            public double Dose { get; set; }
            public int ProductUnitPriceId { get; set; }
            public int? Frequency { get; set; }
            public string Use { get; set; }
            public string ErrorProduct { get; set; }
            public string ErrorBrand { get; set; }
            public string ErrorActiveSubstance { get; set; }
            public string ErrorSupplier { get; set; }
            public string ErrorProductUnitPrice { get; set; }
            public DateTime CreatedAt { get; set; }
            public ProfileUser CreatedByProfile { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public ProfileUser UpdatedByProfile { get; set; }
        }

        public class SamplePrescriptionDetailFilter
        {
            public double? BodyWeight { get; set; }
            public int? DateUse { get; set; }
        }

        public class SamplePrescriptionDetailForStaff
        {
            public int Id { get; set; }
            public int SamplePrescriptionId { get; set; }
            public int ProductId { get; set; }
            public double? DosePerTime { get; set; }
            public double? DosePerDay { get; set; }
            public double? TotalDose { get; set; }
            public int? UnitDoseId { get; set; }
            public int? Quantity { get; set; }
            public int? UnitQuantityId { get; set; }
            public string Use { get; set; }
        }

        public class SamplePrescriptionDetailForCreation
        {
            [Required]
            public int SamplePrescriptionId { get; set; }
            [Required]
            public int ProductId { get; set; }
            [Required]
            public double Dose { get; set; }
            [Required]
            public int ProductUnitPriceId { get; set; }
            public int? Frequency { get; set; }
            public string Use { get; set; }
        }

        public class SamplePrescriptionDetailForUpdate
        {
            [Required]
            public int ProductId { get; set; }
            [Required]
            public double Dose { get; set; }
            [Required]
            public int ProductUnitPriceId { get; set; }
            public int? Frequency { get; set; }
            public string Use { get; set; }
        }
    }
}

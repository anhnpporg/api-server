using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class SamplePrescriptionDetail
    {
        public int Id { get; set; }
        public int SamplePrescriptionId { get; set; }
        public int ProductId { get; set; }
        public double Dose { get; set; }
        public int ProductUnitPriceId { get; set; }
        public int? Frequency { get; set; }
        public string Use { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual UserAccount CreatedByNavigation { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductUnitPrice ProductUnitPrice { get; set; }
        public virtual SamplePrescription SamplePrescription { get; set; }
        public virtual UserAccount UpdatedByNavigation { get; set; }
    }
}

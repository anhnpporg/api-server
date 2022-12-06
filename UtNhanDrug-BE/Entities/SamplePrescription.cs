using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class SamplePrescription
    {
        public SamplePrescription()
        {
            SamplePrescriptionDetails = new HashSet<SamplePrescriptionDetail>();
        }

        public int Id { get; set; }
        public int DiseaseId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }

        public virtual UserAccount CreatedByNavigation { get; set; }
        public virtual UserAccount DeletedByNavigation { get; set; }
        public virtual Disease Disease { get; set; }
        public virtual UserAccount UpdatedByNavigation { get; set; }
        public virtual ICollection<SamplePrescriptionDetail> SamplePrescriptionDetails { get; set; }
    }
}

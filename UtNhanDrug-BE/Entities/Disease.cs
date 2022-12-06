using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Disease
    {
        public Disease()
        {
            SamplePrescriptions = new HashSet<SamplePrescription>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }

        public virtual UserAccount CreatedByNavigation { get; set; }
        public virtual UserAccount DeletedByNavigation { get; set; }
        public virtual UserAccount UpdatedByNavigation { get; set; }
        public virtual ICollection<SamplePrescription> SamplePrescriptions { get; set; }
    }
}

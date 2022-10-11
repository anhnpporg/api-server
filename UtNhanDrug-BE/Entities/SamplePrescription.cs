using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class SamplePrescription
    {
        public int Id { get; set; }
        public int DiseaseId { get; set; }
        public decimal? CustomerWeight { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Manager CreatedByNavigation { get; set; }
        public virtual Disease Disease { get; set; }
        public virtual Manager UpdatedByNavigation { get; set; }
    }
}

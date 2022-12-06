using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static UtNhanDrug_BE.Models.ProfileUserModel.ProfileUserViewModel;

namespace UtNhanDrug_BE.Models.SamplePrescriptionModel
{
    public class SamplePrescriptionViewModel
    {
        public class SamplePrescriptionForManager
        {
            public int Id { get; set; }
            public int DiseaseId { get; set; }
            public string Name { get; set; }
            public DateTime CreatedAt { get; set; }
            public ProfileUser CreatedByProfile { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public ProfileUser UpdatedByProfile { get; set; }
        }

        public class SamplePrescriptionForStaff
        {
            public int Id { get; set; }
            public int DiseaseId { get; set; }
            public string Name { get; set; }
        }

        public class SamplePrescriptionForCreation
        {
            [Required]
            public int DiseaseId { get; set; }
            private string name;
            [Required]
            public string Name
            {
                get { return name.Trim(); }
                set { name = value; }
            }
        }

        public class SamplePrescriptionForUpdate
        {
            private string name; // field
            [Required]
            public string Name   // property
            {
                get { return name.Trim(); }
                set { name = value; }
            }
        }

    }
}

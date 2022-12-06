using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static UtNhanDrug_BE.Models.ProfileUserModel.ProfileUserViewModel;

namespace UtNhanDrug_BE.Models.DiseaseModel
{
    public class DiseaseViewModel
    {
        public class DiseaseForManager
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedAt { get; set; }
            public ProfileUser CreatedByProfile { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public ProfileUser UpdatedByProfile { get; set; }
        }

        public class DiseaseForStaff
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DiseaseForCreation
        {
            private string name;
            [Required]
            public string Name
            {
                get { return name.Trim(); }
                set { name = value; }
            }
        }

        public class DiseaseForUpdate
        {
            private string name;
            [Required]
            public string Name
            {
                get { return name.Trim(); }
                set { name = value; }
            }
        }
    }
}

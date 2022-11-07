using System;

namespace UtNhanDrug_BE.Models.StaffModel
{
    public class UpdateStaffBaseModel
    {
        public string Avartar { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsMale { get; set; }
        
    }
}

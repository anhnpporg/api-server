using System;

namespace UtNhanDrug_BE.Models.StaffModel
{
    public class UpdateStaffModel
    {
        public string Avatar { get; set; }
        public DateTime? Dob { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsMale { get; set; }
        public string Fullname { get; set; }
        public string EmailAddressRecovery { get; set; }
    }
}

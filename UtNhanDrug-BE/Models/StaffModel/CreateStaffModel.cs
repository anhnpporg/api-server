using System;

namespace UtNhanDrug_BE.Models.StaffModel
{
    public class CreateStaffModel
    {
        public string LoginName { get; set; }
        //public string Password { get; set; }
        //public string PasswordConfirm { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public DateTime Dob { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsMale { get; set; }
    }
}

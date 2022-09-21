using System;

namespace UtNhanDrug_BE.Models.UserModel
{
    public class UpdateUserModel
    {
        public string Fullname { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Avatar { get; set; }
        public string PhoneNumber { get; set; }
        public int? GenderId { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}

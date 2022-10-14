using System;
using UtNhanDrug_BE.Entities;

namespace UtNhanDrug_BE.Models.StaffModel
{
    public class ViewStaffModel
    {
        public int UserId { get; set; }
        public string Avatar { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public bool? IsMale { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }
    }
}

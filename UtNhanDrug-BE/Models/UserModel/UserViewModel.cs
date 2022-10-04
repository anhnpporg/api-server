using System;

namespace UtNhanDrug_BE.Models.UserModel
{
    public class UserViewModel
    {
        public string Fullname { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}

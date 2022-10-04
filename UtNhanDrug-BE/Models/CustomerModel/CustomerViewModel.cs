using System;
using UtNhanDrug_BE.Entities;

namespace UtNhanDrug_BE.Models.CustomerModel
{
    public class CustomerViewModel
    {
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Fullname { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }
    }
}

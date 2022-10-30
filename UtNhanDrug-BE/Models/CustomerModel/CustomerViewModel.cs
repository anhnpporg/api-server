using System;
using UtNhanDrug_BE.Entities;

namespace UtNhanDrug_BE.Models.CustomerModel
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public double TotalPoint { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}

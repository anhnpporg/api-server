using System;

namespace UtNhanDrug_BE.Models.CustomerModel
{
    public class CreateCustomerModel
    {
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public double TotalPoint { get; set; }
        public int CreatedBy { get; set; }
    }
}

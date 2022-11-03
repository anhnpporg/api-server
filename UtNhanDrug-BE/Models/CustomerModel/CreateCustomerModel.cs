using System;
using System.ComponentModel.DataAnnotations;

namespace UtNhanDrug_BE.Models.CustomerModel
{
    public class CreateCustomerModel
    {
        [Required]
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
    }
}

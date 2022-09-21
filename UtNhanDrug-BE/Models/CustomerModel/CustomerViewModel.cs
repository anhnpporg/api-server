using System;
using UtNhanDrug_BE.Entities;

namespace UtNhanDrug_BE.Models.CustomerModel
{
    public class CustomerViewModel
    {
        public string PhoneNumber { get; set; }

        public virtual User User { get; set; }
    }
}

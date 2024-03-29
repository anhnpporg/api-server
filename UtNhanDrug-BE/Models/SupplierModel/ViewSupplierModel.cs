﻿using System;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Models.SupplierModel
{
    public class ViewSupplierModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}

﻿using System;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Models.ShelfModel
{
    public class ViewShelfModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public ViewModel CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
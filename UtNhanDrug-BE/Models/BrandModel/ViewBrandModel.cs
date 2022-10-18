using System;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Models.BrandModel
{
    public class ViewBrandModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public ViewModel CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ViewModel? UpdatedBy { get; set; }
    }

}

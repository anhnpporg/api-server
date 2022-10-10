using System;

namespace UtNhanDrug_BE.Models.ActiveSubstanceModel
{
    public class ViewActiveSubstanceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}

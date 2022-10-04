using System;
using UtNhanDrug_BE.Entities;

namespace UtNhanDrug_BE.Models.ManagerModel
{
    public class ManagerViewModel
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }
    }
}

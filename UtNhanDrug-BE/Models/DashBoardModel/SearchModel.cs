using System;
using System.ComponentModel.DataAnnotations;

namespace UtNhanDrug_BE.Models.DashBoardModel
{
    public class SearchModel
    {
        [Required]
        public bool ByDay { get; set; }
        [Required]
        public bool ByMonth { get; set; }
        [Required]
        public bool ByYear { get; set; }
        [Required]
        public int Size { get; set; }
    }
}

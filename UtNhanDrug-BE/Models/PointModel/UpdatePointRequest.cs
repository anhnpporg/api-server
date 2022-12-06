using System.ComponentModel.DataAnnotations;

namespace UtNhanDrug_BE.Models.PointModel
{
    public class UpdatePointRequest
    {
        [Required]
        public double ToPoint { get; set; }
        [Required]
        public int ToMoney { get; set; }
    }
}

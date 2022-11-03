using System.ComponentModel.DataAnnotations;

namespace UtNhanDrug_BE.Models.ShelfModel
{
    public class CreateShelfModel
    {
        [Required]
        public string Name { get; set; }
    }
}

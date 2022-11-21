using System.ComponentModel.DataAnnotations;

namespace UtNhanDrug_BE.Models.ManagerModel
{
    public class UpdateManagerModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace UtNhanDrug_BE.Models.UserLoginModel
{
    public class ChangePasswordByIdModel
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}

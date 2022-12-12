using System.ComponentModel.DataAnnotations;

namespace UtNhanDrug_BE.Models.UserLoginModel
{
    public class ChangePasswordModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string TokenRecovery { get; set; }
    }
}

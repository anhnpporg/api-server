using System.ComponentModel.DataAnnotations;

namespace UtNhanDrug_BE.Models.UserModel
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}

using UtNhanDrug_BE.Entities;

namespace UtNhanDrug_BE.Models.ManagerModel
{
    public class ManagerViewModel
    {
        public string Email { get; set; }
        public bool IsAdmin { get; set; }

        public virtual User User { get; set; }
    }
}

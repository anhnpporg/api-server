using UtNhanDrug_BE.Entities;

namespace UtNhanDrug_BE.Models.StaffModel
{
    public class ViewStaffModel
    {
        public string Email { get; set; }

        public virtual User User { get; set; }
    }
}

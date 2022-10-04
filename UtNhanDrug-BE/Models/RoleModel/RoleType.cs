using System.ComponentModel;

namespace UtNhanDrug_BE.Models.RoleModel
{
    public class RoleType
    {
        [DefaultValue("MANAGER")]
        public string Manager { get; }
        [DefaultValue("STAFF")]
        public string Staff { get; }
    }
}

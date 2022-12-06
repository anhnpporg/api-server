using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UtNhanDrug_BE.Models.ProfileUserModel
{
    public class ProfileUserViewModel
    {
        public class ProfileUser
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public bool? IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}

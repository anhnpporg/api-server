using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class HashingAlgorithm
    {
        public HashingAlgorithm()
        {
            UserLoginData = new HashSet<UserLoginDatum>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserLoginDatum> UserLoginData { get; set; }
    }
}

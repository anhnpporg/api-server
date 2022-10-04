using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class User
    {
        public User()
        {
            Customers = new HashSet<Customer>();
            Managers = new HashSet<Manager>();
            UserLoginData = new HashSet<UserLoginDatum>();
            UserLoginDataExternals = new HashSet<UserLoginDataExternal>();
            staff = new HashSet<Staff>();
        }

        public int Id { get; set; }
        public string Fullname { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Manager> Managers { get; set; }
        public virtual ICollection<UserLoginDatum> UserLoginData { get; set; }
        public virtual ICollection<UserLoginDataExternal> UserLoginDataExternals { get; set; }
        public virtual ICollection<Staff> staff { get; set; }
    }
}

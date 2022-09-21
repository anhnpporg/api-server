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
            staff = new HashSet<Staff>();
        }

        public int Id { get; set; }
        public string Fullname { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Avatar { get; set; }
        public string PhoneNumber { get; set; }
        public int? GenderId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsBan { get; set; }
        public DateTime? BanDate { get; set; }

        public virtual Gender Gender { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Manager> Managers { get; set; }
        public virtual ICollection<Staff> staff { get; set; }
    }
}

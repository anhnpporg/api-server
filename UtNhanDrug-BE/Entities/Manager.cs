using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class Manager
    {
        public Manager()
        {
            ActiveSubstanceCreatedByNavigations = new HashSet<ActiveSubstance>();
            ActiveSubstanceUpdatedByNavigations = new HashSet<ActiveSubstance>();
            BrandCreatedByNavigations = new HashSet<Brand>();
            BrandUpdatedByNavigations = new HashSet<Brand>();
            CategoryCreatedByNavigations = new HashSet<Category>();
            CategoryUpdatedByNavigations = new HashSet<Category>();
            ProductCreatedByNavigations = new HashSet<Product>();
            ProductUpdatedByNavigations = new HashSet<Product>();
        }

        public int Id { get; set; }
        public int UserAccountId { get; set; }

        public virtual UserAccount UserAccount { get; set; }
        public virtual ICollection<ActiveSubstance> ActiveSubstanceCreatedByNavigations { get; set; }
        public virtual ICollection<ActiveSubstance> ActiveSubstanceUpdatedByNavigations { get; set; }
        public virtual ICollection<Brand> BrandCreatedByNavigations { get; set; }
        public virtual ICollection<Brand> BrandUpdatedByNavigations { get; set; }
        public virtual ICollection<Category> CategoryCreatedByNavigations { get; set; }
        public virtual ICollection<Category> CategoryUpdatedByNavigations { get; set; }
        public virtual ICollection<Product> ProductCreatedByNavigations { get; set; }
        public virtual ICollection<Product> ProductUpdatedByNavigations { get; set; }
    }
}

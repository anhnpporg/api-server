using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class DataSettingSystemCustomerPoint
    {
        public int Id { get; set; }
        public double ToPoint { get; set; }
        public int ToMoney { get; set; }
    }
}

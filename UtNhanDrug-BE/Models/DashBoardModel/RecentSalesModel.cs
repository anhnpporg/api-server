using System;
using UtNhanDrug_BE.Models.ModelHelper;

namespace UtNhanDrug_BE.Models.DashBoardModel
{
    public class RecentSalesModel
    {
        public int Id { get; set; }
        public ViewModel Customer { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreateAt { get; set; }
    }
}

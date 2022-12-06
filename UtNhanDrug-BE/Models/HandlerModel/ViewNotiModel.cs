using System;
using System.Collections.Generic;

namespace UtNhanDrug_BE.Models.HandlerModel
{
    public class ViewNotiModel
    {
        public string Title { get; set; }
        public List<ListNoti> ListNoti { get; set; } 
    }
    public class ListNoti
    {
        public int Id { get; set; }
        public int? BatchId { get; set; }
        public int? ProductId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

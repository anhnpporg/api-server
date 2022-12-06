using System;

namespace UtNhanDrug_BE.Models.HandlerModel
{
    public class SaveNotiRequest
    {
        public int? BatchId { get; set; }
        public int? ProductId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}

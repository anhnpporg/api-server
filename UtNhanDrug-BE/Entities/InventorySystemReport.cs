using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class InventorySystemReport
    {
        public int Id { get; set; }
        public int InventorySystemReportTypeId { get; set; }
        public int ConsignmentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }

        public virtual Consignment Consignment { get; set; }
        public virtual Manager CreatedByNavigation { get; set; }
        public virtual InventorySystemReportType InventorySystemReportType { get; set; }
    }
}

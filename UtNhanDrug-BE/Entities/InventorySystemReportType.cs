using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class InventorySystemReportType
    {
        public InventorySystemReportType()
        {
            InventorySystemReports = new HashSet<InventorySystemReport>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<InventorySystemReport> InventorySystemReports { get; set; }
    }
}

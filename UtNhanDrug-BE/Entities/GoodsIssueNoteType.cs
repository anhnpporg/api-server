using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class GoodsIssueNoteType
    {
        public GoodsIssueNoteType()
        {
            GoodsIssueNotes = new HashSet<GoodsIssueNote>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<GoodsIssueNote> GoodsIssueNotes { get; set; }
    }
}

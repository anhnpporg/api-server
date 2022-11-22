using System.Collections.Generic;

namespace UtNhanDrug_BE.Models.DashBoardModel
{
    public class ChartModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<double> Data { get; set; }
    }
}

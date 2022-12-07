using System;
using System.Collections.Generic;

namespace UtNhanDrug_BE.Models.DashBoardModel
{
    public class ChartModel
    {
        public List<Line> ListLine { get; set; }
        public List<DateChart> ListDate { get; set; }
    }
    public class Line
    {
        public string Name { get; set; } 
        public string Type { get; set; }
        public List<Decimal> Data { get; set; }
    }
    //public class Number
    //{
    //    public decimal Num {get; set;}
    //}
    public class DateChart
    {
        public DateTime Date { get; set; }
    }
}

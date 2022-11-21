using System;

namespace UtNhanDrug_BE.Hepper
{
    public class LocalDateTime
    {
        public static DateTime DateTimeNow()
        {
            DateTime todaySystem = DateTime.Now;
            var todayVN = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(todaySystem, "SE Asia Standard Time");
            return todayVN;
        }
    }
}

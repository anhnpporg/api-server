using System;

namespace UtNhanDrug_BE.Hepper
{
    public class CheckMonday
    {
        public static int MondayOfWeek(DateTime date)
        {
            var dayOfWeek = date.DayOfWeek;

            if (dayOfWeek == DayOfWeek.Sunday)
            {
                //xét chủ nhật là đầu tuần thì thứ 2 là ngày kế tiếp nên sẽ tăng 1 ngày  
                //return date.AddDays(1);  

                // nếu xét chủ nhật là ngày cuối tuần  
                return 6;
            }

            // nếu không phải thứ 2 thì lùi ngày lại cho đến thứ 2  
            return dayOfWeek - DayOfWeek.Monday;

        }
    }
}

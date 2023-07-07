using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Shared.Utils
{
    public class DatetimeUtil
    {
        /// <summary> 
        /// 
        /// </summary> 
        /// <param name="timeStamp"></param> 
        /// <returns></returns> 
        public static DateTime StampToDateTime(Double timeStamp)
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp.ToString() + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);


            return dateTimeStart.Add(toNow);
        }


        /// <summary> 
        /// DateTime 
        /// </summary> 
        /// <param name="time"></param> 
        /// <returns></returns> 
        public static string DateTimeToStamp(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (time - startTime).TotalMilliseconds.ToString();
        }

        public static DateTime DateTimeToStamp(string time)
        {
            return Convert.ToDateTime(time);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class DateTimeExtentsion
    {
        public static long MillisecondsStamp(this DateTime datetime)
        {
            return new DateTimeOffset(datetime).ToUnixTimeMilliseconds();
        }
        public static long SecondsStamp(this DateTime datetime)
        {
            return new DateTimeOffset(datetime).ToUnixTimeSeconds();
        }
        public static DateTime SecondsToTime(this long datetime)
        {
            DateTime dtStart = new DateTime(1970, 1, 1);
            return dtStart.AddSeconds(datetime);
        }
        public static DateTime MillisecondsToTime(this long datetime)
        {
            DateTime dtStart = new DateTime(1970, 1, 1);
            return dtStart.AddMilliseconds(datetime).ToLocalTime();
        }
    }
}

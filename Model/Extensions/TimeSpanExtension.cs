using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Extensions
{
    public static class TimeSpanExtension
    {

        public static TimeSpan LocalTimeSpanToUTC(this TimeSpan ts, int offsetInMinutes)
        {
            DateTime dt = DateTime.Now.Date.Add(ts);
            DateTime dtUtc = dt.AddMinutes(-offsetInMinutes);
            TimeSpan tsUtc = dtUtc.TimeOfDay;
            return tsUtc;
        }

        public static TimeSpan UTCTimeSpanToLocal(this TimeSpan tsUtc, int offsetInMinutes)
        {
            DateTime dtUtc = DateTime.UtcNow.Date.Add(tsUtc);
            DateTime dt = dtUtc.AddMinutes(offsetInMinutes);
            TimeSpan ts = dt.TimeOfDay;
            return ts;
        }
    }
}

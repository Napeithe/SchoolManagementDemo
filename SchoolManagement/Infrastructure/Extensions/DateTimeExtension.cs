using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Infrastructure.Extensions
{
    public static class DateTimeExtension
    {
        public static string AvailableToShifted(this DateTime date)
        {
            return $"{date:s}Z";
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dob)
        {
            var today = DateTime.Now;
            int age = today.Year - dob.Year;
            if (dob.Date > today.Date.AddYears(-age))
                age--;

            return age;
        }
    }
}
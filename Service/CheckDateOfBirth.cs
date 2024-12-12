using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supermarket_System.Service
{
    public static class CheckDateOfBirth
    {
       public static bool IsDateOfBirth(DateTime dateOfBirth)
        {
            int minimumAge = 18;
            DateTime today = DateTime.Today;
            int Age = today.Year - dateOfBirth.Year;
            if(dateOfBirth > today.AddYears(-Age))
            {
                Age--;

            }
            return Age >= minimumAge && dateOfBirth <= today;
        }
    }
}
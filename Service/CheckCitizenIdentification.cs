using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Supermarket_System.Service
{
    public static class CheckCitizenIdentification
    {
        public static bool CheckCCCD(string cccd)
        {
            if (!cccd.All(char.IsDigit))
            {
                return false;
            }
            if(cccd.Length == 12 && Regex.IsMatch(cccd, @"^\d{12}$"))
            {
                return true;
            }
            return false;
        }
    }
}
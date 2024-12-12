using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Supermarket_System.Service
{
    public static  class CheckAddress
    {
        public static bool IsAddress(string address)
        {
            // Ví dụ: Biểu thức chính quy đơn giản cho địa chỉ ở Mỹ
            string pattern = @"^\d+\s+\w+\s+[\w\s]+,\s+\w+\s+(\d{5}-\d{4}|\d{5})$";
            return !string.IsNullOrEmpty(address) && Regex.IsMatch(address, pattern);
        }
    }
}
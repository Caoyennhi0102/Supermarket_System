using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Supermarket_System.Service
{
    public static class CheckPhoneNumber
    {
        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            if (!phoneNumber.All(char.IsDigit))
            {
                return false;
            }
            // Định dạng số điện thoại: Bắt đầu bằng 0, dài 10-11 ký tự
            string mobilePattern = @"^0\d{9,10}$";

            // Định dạng cho số điện thoại cố định Việt Nam (ví dụ)
            string fixedLinePattern = @"^\(0\d{2}\)\d{7,8}$";

            // Kiểm tra tính hợp lệ với Regular Expression
            return Regex.IsMatch(phoneNumber, mobilePattern) || Regex.IsMatch(phoneNumber, fixedLinePattern);
        }

    }
}
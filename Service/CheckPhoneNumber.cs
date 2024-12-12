using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supermarket_System.Service
{
    public static class CheckPhoneNumber
    {
        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            // Định dạng số điện thoại: Bắt đầu bằng 0, dài 10-11 ký tự
            string pattern = @"^0\d{9,10}$";

            // Kiểm tra tính hợp lệ với Regular Expression
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, pattern);
        }

    }
}
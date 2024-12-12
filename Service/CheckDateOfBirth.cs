using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supermarket_System.Service
{
    public static class CheckDateOfBirth
    {
        public static bool IsDateOfBirth(DateTime dateOfBirth, int minimumAge = 18)
        {
            try
            {
                // Kiểm tra ngày sinh có hợp lệ không (ví dụ: ngày, tháng, năm hợp lý)
                if (!dateOfBirth.Date.Equals(dateOfBirth))
                {
                    throw new ArgumentException("Ngày sinh không hợp lệ.");
                }

                // Tính tuổi
                int age = DateTime.Today.Year - dateOfBirth.Year;
                if (dateOfBirth > DateTime.Today.AddYears(-age))
                {
                    age--;
                }

                // Kiểm tra tuổi và trả về kết quả
                return age >= minimumAge;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                Console.WriteLine("Lỗi khi kiểm tra ngày sinh: " + ex.Message);
                return false;
            }
        }
    }
}
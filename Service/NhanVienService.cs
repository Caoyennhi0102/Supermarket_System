using Supermarket_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;


namespace Supermarket_System.Service
{
    public class NhanVienService
    {
        private readonly SqlConnectionServer _sqlConnectionServer;
        public NhanVienService(SqlConnectionServer sqlConnectionServer)
        {
            _sqlConnectionServer = sqlConnectionServer;
        }
        public string LoginService(string username, string password, out bool isFirstLogin)
        {
            isFirstLogin = false;
            var nhanvien = _sqlConnectionServer.NhanViens.FirstOrDefault(u => u.UserName == username);
            if(nhanvien == null)
            {
                return "Tài khoản không tồn tại.";
            }
            if (nhanvien.Locked)
            {
                return "Tài khoản đã bị khóa.";
            }

            if(!VerifyPassword(password, nhanvien.PasswordHash))
            {
                nhanvien.SoLanDangNhapSai = (nhanvien.SoLanDangNhapSai ?? 0) + 1;
                if(nhanvien.SoLanDangNhapSai >= 3)
                {
                    nhanvien.Locked = true;
                    _sqlConnectionServer.SaveChanges();
                    return "Bạn đã nhập sai mật khẩu 3 lần. Tài khoản đã bị khóa.";
                }
                _sqlConnectionServer.SaveChanges();
                return $"Mật khẩu không đúng.Bạn còn {3 - nhanvien.SoLanDangNhapSai}lần thử.";

            }
            nhanvien.SoLanDangNhapSai = 0;
            nhanvien.ThoiGianDangNhap = DateTime.Now;
            if(nhanvien.DangNhapLanDau == true)
            {
                isFirstLogin = true;
            }
            _sqlConnectionServer.SaveChanges();
            return "Đăng nhập thành công!";
        }
        private bool VerifyPassword(string password, string storedHash)
        {
            // Logic so khớp mật khẩu với hashing
            // Ví dụ: sử dụng BCrypt hoặc một hàm băm khác
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
        public NhanVien GetUserByUserName(string username)
        {
            return _sqlConnectionServer.NhanViens.FirstOrDefault(u => u.UserName == username);
        }
        public string GetUserRole(string username)
        {
            var user = GetUserByUserName(username);
            return user != null ? user.Roles : string.Empty;
        }
    }
}
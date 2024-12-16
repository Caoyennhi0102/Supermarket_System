using Supermarket_System.Models;
using Supermarket_System.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Supermarket_System.Controllers
{
    public class LoginController : Controller
    {
        private readonly NhanVienService _nhanVienService;
        public LoginController()
        {
            var sqlConnectionServer = new SqlConnectionServer();
            _nhanVienService = new NhanVienService(sqlConnectionServer);
        }
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Login");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, NhanVien nhanVien)
        {
            bool isFirstLogin;
            string result = _nhanVienService.LoginService(username, password, out isFirstLogin);
            
            if(result == "Đăng Nhập Thành Công")
            {
                Session["UserName"] = nhanVien.TenNV;
                Session["MaNV"] = nhanVien.MaNV;
                if(nhanVien.Avatar != null)
                {
                    string avatarBase64 = Convert.ToBase64String(nhanVien.Avatar);
                    string imageDataURL = $"data:image/png;base64,{avatarBase64}";
                    Session["Avatar"] = imageDataURL;
                }
                else
                {
                    Session["Avatar"] = "/Images/default-avatar.png";
                }

                FormsAuthentication.SetAuthCookie(username, false);
                if (isFirstLogin)
                {
                    return RedirectToAction("ChangeCredentials", "Index");
                }
                var userRole = _nhanVienService.GetUserRole(username);
                if(userRole == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }else if(userRole == "Manager")
                {
                    return RedirectToAction("Dashboard", "Manager");
                }
                else
                {
                    return RedirectToAction("Dashboard", "User");
                }
            }
            ViewBag.ErrorMessage = result;
            return View();
        }
        [HttpGet]
        public ActionResult ChangeCredentials()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangeCredentials(string newusername, string newpassword)
        {
            var currentUser = Session["UserName"]?.ToString();
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToAction("Login", "ChangeCredentials");
            }
            using(var context = new SqlConnectionServer())
            {
                var user = context.NhanViens.FirstOrDefault(u => u.UserName == newusername);
                if( user == null)
                {
                    return RedirectToAction("Login", "Index");
                }
                user.UserName = newusername;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newpassword);
                user.DangNhapLanDau = false;
                context.SaveChanges();
            }
            return RedirectToAction("Login", "Inddex");
        }
    }
}
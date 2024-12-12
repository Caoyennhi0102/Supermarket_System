using Supermarket_System.Models;
using Supermarket_System.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Supermarket_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }
        public AdminController() { }
        // GET: Admin
        [HttpGet]
        public ActionResult AddEmployee()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult AddEmployee(NhanVien nhanVien, int sothuTu, string maQl, string role, string status)
        {
            try
            {
                // Gọi phương thức từ AdminService
                var reult = _adminService.AddNhanVien(nhanVien, sothuTu, maQl, role, status);

                if (string.IsNullOrWhiteSpace(reult))
                {
                    ViewBag.ErrorMessage = "Không có thông tin phản hồi từ hệ thống.";
                    return View("AddEmployee");
                }

                if (reult.StartsWith("Lỗi"))
                {
                    ViewBag.ErrorMessage = reult;
                    return View("AddEmployee");
                }

                TempData["SuccessMessage"] = reult;
                return RedirectToAction("NhanVienList");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
                return View("AddEmployee");
            }
        }
    }
}
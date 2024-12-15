using Supermarket_System.Models;
using Supermarket_System.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Supermarket_System.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
       
        public AdminController()
        {
            _adminService = new AdminService(new SqlConnectionServer());
            
        }
        public ActionResult Dashboard()
        {
            return Json(new { message = "Welcome to Admin Dashboard" });
        }
        public ActionResult logout()
        {
            bool result = _adminService.LogOutAsync();
            if (result)
            {
                return RedirectToAction("Admin", "Login");
            }
            else
            {
                return View("Admin");
            }
        }

        // GET: Admin
        [HttpGet]
        public ActionResult AddEmployee()
        {
            var nhanvien = _adminService.GetAllEmployeest();
            ViewBag.employeeList = nhanvien;
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
                    return Json(new { success = false, message = "Không có thông tin phản hồi từ hệ thống." });
                }

                if (reult.StartsWith("Lỗi"))
                {
                    return Json(new { success = false, message = reult });
                }
                return Json(new
                {
                    success = true,
                    nhanVien = new
                    {
                         nhanVien.MaNV,
                         nhanVien.MaBP,
                         nhanVien.MaChucVu,
                         nhanVien.TenNV,
                         nhanVien.CCCD,
                         nhanVien.GioiTinh,
                         nhanVien.NgaySinh,
                         nhanVien.DiaChi,
                         nhanVien.SDT,
                         nhanVien.Email,
                         nhanVien.Roles,
                         nhanVien.UserName,
                         nhanVien.TrangThaiTaiKhoan,
                         nhanVien.NgayTao
                         
                        

                    }
                });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi: {ex.Message}" });
            }
        }
        [HttpPost]
        public JsonResult SearchEmployee(string maNV)
        {
            // Kiểm tra nếu mã nhân viên rỗng
            if (string.IsNullOrEmpty(maNV))
            {
                return Json(new { success = false, message = "Mã nhân viên không hợp lệ." });
            }
            var nhanvien = _adminService.SearchEmployeeByMaNhanVien(maNV);
            if(nhanvien == null)
            { // Trả về một thông báo lỗi trong view
                return Json(new { success = false, message = "Không tìm thấy nhân viên với mã nhân viên đã cung cấp." });
            }
            return Json(new { success = true, employee = nhanvien });
        }
        
        [HttpGet]
        public ActionResult UpdateEmployee()
        {
            return View();
        }
        [HttpPost]
        public ActionResult  UpdateEmployee(NhanVien updatenhanVien, string maQL, string status)
        {
            try
            {
                string   result = _adminService.UpdateNhanVien(updatenhanVien, maQL, status);
                if(result != "success")
                {
                    return Json(new { success = false, message = result});
                }
                return Json(new { success = true, message = "Cập nhật thông tin nhân viên thành công." });

            }catch(Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi khi cập nhật thông tin: {ex.Message}" });
            }
        }
    }
}
﻿using Microsoft.AspNetCore.Http;
using Supermarket_System.Models;
using Supermarket_System.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;

namespace Supermarket_System.Controllers
{
  //  [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        private readonly SqlConnectionServer _sqlConnectionServer;
        public AdminController(SqlConnectionServer sqlConnectionServer)
        {
            _adminService = new AdminService(new SqlConnectionServer());
            _sqlConnectionServer = sqlConnectionServer;
        }
        public AdminController() { }
      /*  protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserName"] == null)
            {
                filterContext.Result = RedirectToAction("Login", "Login");
            }
            base.OnActionExecuting(filterContext);
        }*/
        public ActionResult Dashboard()
        {
            bool IsSessionActive = (Session["UserName"] != null);
            ViewBag.IsSessionActive = IsSessionActive;
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "Admin");
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
        public async Task <ActionResult> AddEmployee(NhanVien nhanVien, int sothuTu, string maQl, string role, string status, IFormFile formFile)
        {
            try
            {
                if(formFile != null && formFile.Length > 0)
                {
                    using(var memoryStream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(memoryStream);
                        nhanVien.Avatar = memoryStream.ToArray();
                    }
                }
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
        [HttpGet]
        public ActionResult DeleteEmployee()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DeleteEmployee(string maNV, string MaQL)
        {
            try
            {
                if(string.IsNullOrEmpty(maNV) || string.IsNullOrEmpty(MaQL))
                {
                    return Json(new { success = false, message = "Mã nhân viên hoặc mã quản lý không hợp lệ." });
                }
                var  result = _adminService.DeleteNhanVien(maNV, MaQL);
                if (result != "Nhân viên đã được xóa thành công.")
                {
                    return Json(new { success = false, message = result});

                }
                return Json(new { success = true, message = "Nhân viên đã được xóa thành công!" });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi xóa nhân viên{ex.Message}" });
            }
        }
    }
}
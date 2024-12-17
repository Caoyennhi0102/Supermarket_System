using Microsoft.AspNetCore.Http;
using Supermarket_System.Models;
using Supermarket_System.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            var boPhans = _sqlConnectionServer.BoPhans.ToList();
            ViewBag.MaBP = new SelectList(boPhans, "MaBP", "TenBoPhan");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                
                var boPhans = _sqlConnectionServer.BoPhans.ToList();
                
                ViewBag.MaBP = new SelectList(boPhans, "MaBP", "TenBoPhan");

                
                
                // Gọi phương thức từ AdminService
                var result = _adminService.AddNhanVien(nhanVien, sothuTu, maQl, role, status);

                if (string.IsNullOrWhiteSpace(result))
                {
                    ModelState.AddModelError("", "Không có thông tin phản hồi từ hệ thống.");
                    return View(nhanVien);
                }

                if (result.StartsWith("Lỗi"))
                {
                    ModelState.AddModelError("", result);
                    return View(nhanVien);
                }

                return Json(new { success = true, message = "Thêm nhân viên thành công" });
           

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Đã xảy ra lỗi: {ex.Message}");

                // Truyền lại ViewBag nếu có lỗi
                var boPhans = _sqlConnectionServer.BoPhans.ToList();
                ViewBag.MaBP = new SelectList(boPhans, "MaBP", "TenBoPhan");
                return View(nhanVien);
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
            var boPhans = _sqlConnectionServer.BoPhans.ToList();
            ViewBag.MaBP = new SelectList(boPhans, "MaBP", "TenBoPhan");
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
            var boPhans = _sqlConnectionServer.BoPhans.ToList();
            ViewBag.MaBP = new SelectList(boPhans, "MaBP", "TenBoPhan");
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
        // View thêm bộ phận 
        [HttpGet]
        public ActionResult AddDepartment()
        {
            return View();
        }
        // Controller thêm bộ phận 
        [HttpPost]
        public ActionResult AddDepartment(string TenBP)
        {
            try
            {
                if (string.IsNullOrEmpty(TenBP))
                {
                    return Json(new { success = false, message = "Tên Bộ Phận Không Được Để Trống" });
                }
                var Department = _adminService.AddParts(TenBP);
                if(Department)
                {
                    return Json(new { success = true, message = "Thêm bộ phận thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Thêm bộ phận không thành công" });
                }
                
            }
            catch(Exception ex)
            {
                return Json(new { success = false, mesage = $"Có lỗi trong quá trình thêm bộ phận{ex.Message}" });
            }
        }
        // Controller lấy danh sách bộ phận thêm vào bảng danh sach bộ phận 
        public ActionResult GetDepartments()
        {
            try
            {
                var getDepartments = _sqlConnectionServer.BoPhans.Select(BP => new { BP.MaBP, BP.TenBoPhan }).ToList();
                return Json(getDepartments, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy trong quá trinh gọi danh sách bộ phận{ex.Message}" });
            }
        }
        public JsonResult GetChucVuByBoPhan(string maBoPhan)
        {
            // Lấy các chức vụ theo mã bộ phận từ cơ sở dữ liệu
            var chucVus = _sqlConnectionServer.ChucVus.Where(cv => cv.MaBP == maBoPhan).ToList();

            // Trả về dữ liệu dưới dạng JSON
            return Json(chucVus, JsonRequestBehavior.AllowGet);
        }
    }
}
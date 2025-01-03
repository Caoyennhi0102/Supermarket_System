﻿using Microsoft.AspNetCore.Http;
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
using System.Web.UI.WebControls.Expressions;
// Trang Controller Admin các chức năng của trang Admin
namespace Supermarket_System.Controllers
{
    // Đánh dấu phân quyền đăng nhập Admin
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
        public async Task<ActionResult> AddEmployee(NhanVien nhanVien, int sothuTu, string maQl, string role, string status, IFormFile formFile)
        {
            try
            {
                if (formFile != null && formFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(memoryStream);
                        nhanVien.Avatar = memoryStream.ToArray();
                    }
                }





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
            if (nhanvien == null)
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
        public ActionResult UpdateEmployee(NhanVien updatenhanVien, string maQL, string status)
        {
            try
            {


                string result = _adminService.UpdateNhanVien(updatenhanVien, maQL, status);
                if (result != "success")
                {
                    return Json(new { success = false, message = result });
                }
                return Json(new { success = true, message = "Cập nhật thông tin nhân viên thành công." });

            } catch (Exception ex)
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
                if (string.IsNullOrEmpty(maNV) || string.IsNullOrEmpty(MaQL))
                {
                    return Json(new { success = false, message = "Mã nhân viên hoặc mã quản lý không hợp lệ." });
                }
                var result = _adminService.DeleteNhanVien(maNV, MaQL);
                if (result != "Nhân viên đã được xóa thành công.")
                {
                    return Json(new { success = false, message = result });

                }
                return Json(new { success = true, message = "Nhân viên đã được xóa thành công!" });
            }
            catch (Exception ex)
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
                if (Department)
                {
                    return Json(new { success = true, message = "Thêm bộ phận thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Thêm bộ phận không thành công" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, mesage = $"Có lỗi trong quá trình thêm bộ phận{ex.Message}" });
            }
        }
        // Controller lấy danh sách bộ phận thêm vào bảng danh sách bộ phận 
        public ActionResult GetDepartments()
        {
            try
            {
                var getDepartments = _sqlConnectionServer.BoPhans.Select(BP => new { BP.MaBP, BP.TenBoPhan }).ToList();
                return Json(getDepartments, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy trong quá trinh gọi danh sách bộ phận{ex.Message}" });
            }
        }
        /*
        public JsonResult GetChucVuByBoPhan(string maBoPhan)
        {
            // Lấy các chức vụ theo mã bộ phận từ cơ sở dữ liệu
            var chucVus = _sqlConnectionServer.ChucVus.Where(cv => cv.MaBP == maBoPhan).ToList();

            // Trả về dữ liệu dưới dạng JSON
            return Json(chucVus, JsonRequestBehavior.AllowGet);
        }
        */
        [HttpGet]
        public ActionResult GetByDepartmentID(string maBP)
        {
            try
            {
                
                if (string.IsNullOrEmpty(maBP))
                {
                    return Json(new { success = false, message = "Mã bộ phận không được để trống" });
                }
                var ListBoPhan = _sqlConnectionServer.BoPhans;
                if(ListBoPhan == null)
                {
                    return Json(new { success = false, message = "Dữ liệu bộ phận không tồn tại." });
                }
                var bophan = _sqlConnectionServer.BoPhans.FirstOrDefault(bp => bp.MaBP == maBP);
                if (bophan != null)
                {
                    return Json(new
                    {
                        success = true,
                        maBP = bophan.MaBP,
                        tenBP = bophan.TenBoPhan
                    }, JsonRequestBehavior.AllowGet); // Bắt buộc phải có AllowGet
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Không tìm thấy bộ phận với mã này."
                    }, JsonRequestBehavior.AllowGet);// Bắt buộc phải có AllowGet
                }
            } 
             catch (Exception ex)
            {
                // Log lỗi chi tiết để kiểm tra
                System.Diagnostics.Debug.WriteLine($"Lỗi: {ex.Message}");

                return Json(new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi truy vấn cơ sở dữ liệu."
                }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpGet]
        public ActionResult UpdateDepartmentName()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UpdateDepartmentName(string maBP, string TenBP)
        {
            try
            {
                if (string.IsNullOrEmpty(maBP) || string.IsNullOrEmpty(TenBP))
                {
                    return Json(new { success = false, message = "Mã bộ phận và tên bộ phận không được để trống." });
                }

                var bophan = _adminService.UpdateParts(maBP, TenBP);
                if (bophan)
                {
                    return Json(new { success = true, message = "Cập nhật tên bộ phận thành công!" });
                }
                return Json(new { success = false, message = "Bộ phận không tồn tại" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi trong quá trình cập nhật tên bộ phận{ex.Message}" });
            }
        }
        [HttpGet]
        public ActionResult DeleteDepartment()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DeleteDepartment(string maBP)
        {
            try
            {
                if (string.IsNullOrEmpty(maBP))
                {
                    return Json(new { success = false, message = "Mã bộ phận và tên bộ phận không được để trống." });
                }
                var bophan = _adminService.DeleteParts(maBP);
                if (bophan)
                {
                    return Json(new { success =true , message = "Xóa bộ phận thành công" });
                    
                }
                return Json(new { success = false, message = "Bộ phận không tồn tại" });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra trong quá trình xóa bộ phận{ex.Message}" });
            }
        }
        [HttpGet]
        public ActionResult AddPosition()
        {
            var bophan = _sqlConnectionServer.BoPhans.ToList();
            ViewBag.Bophans = bophan;
            var ListCV = (from chucvu in _sqlConnectionServer.ChucVus
                          join bophans in _sqlConnectionServer.BoPhans
                          on chucvu.MaBP equals bophans.MaBP
                          select new
                          {
                              chucvu.MaBP,
                              bophans.TenBoPhan,
                              chucvu.MaChucVu,
                              chucvu.TenChucVu
                          }).ToList();
            ViewBag.listCV = ListCV;
            return View();
        }
        [HttpPost]
        public ActionResult AddPosition(string TenCV, string MaBP)
        {
            try
            {

                if(string.IsNullOrEmpty(TenCV) || string.IsNullOrEmpty(MaBP))
                {
                    return Json(new { success = false, message = "Tên chức vụ và bộ phận không được để trống." });
                }
                var resultCV = _adminService.AddPosition(TenCV, MaBP);
                if (resultCV)
                {
                    // Lấy tên bộ phận và mã chức vụ mới đã tạo
                    var boPhan = _sqlConnectionServer.BoPhans.FirstOrDefault(b => b.MaBP == MaBP);
                    var chucVu = _sqlConnectionServer.ChucVus.FirstOrDefault(c => c.TenChucVu == TenCV);

                    return Json(new { success = true, maChucVu = chucVu.MaChucVu, tenBoPhan = boPhan?.TenBoPhan });
                }
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm chức vụ." });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra khi thêm chức vụ.{ex.Message}" });
            }
        }
    }
}
using Microsoft.Ajax.Utilities;
using Supermarket_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;

namespace Supermarket_System.Service
{
    public class AdminService
    {
        private readonly SqlConnectionServer _sqlConnectionServer;
        
        public AdminService(SqlConnectionServer sqlConnectionServer)
        {
            _sqlConnectionServer = sqlConnectionServer;
        }
        public string GenerateMaNhanVien(int STT)
        {
            string DateYear = DateTime.Now.ToString("yyyyMMdd");
            string maNV = $"NV-{DateYear}-{STT.ToString().PadLeft(4, '0')}";
            return maNV;
        }
        public string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public List<NhanVien> GetAllEmployeest()
        {
            return _sqlConnectionServer.NhanViens.ToList();
        }

        public string AddNhanVien(NhanVien nhanvien, int sothutu, string maQL, string role, string status)
        {
            try
            {
                if (!CheckPhoneNumber.IsPhoneNumberValid(nhanvien.SDT))
                {
                    Console.WriteLine($"Số điện thoại không hợp lệ: {nhanvien.SDT}");
                    return "Số điện thoại không hợp lệ.";
                }
                if (!CheckDateOfBirth.IsDateOfBirth(nhanvien.NgaySinh))
                {
                    return "Ngày sinh không hợp lệ. Nhân viên phải trên 18 tuổi và không phải ngày trong tương lai.";
                }
                if (!CheckAddress.IsAddress(nhanvien.DiaChi))
                {
                    
                    return "Địa chỉ không hợp lệ. Địa chỉ phải có ít nhất 5 ký tự.";
                }
                if (!CheckCitizenIdentification.CheckCCCD(nhanvien.CCCD))
                {
                    return "Căn cước công dân không hợp lệ";
                }
                if (!CheckEmail.IsEmail(nhanvien.Email))
                {
                    return "Email không hợp lệ.";
                }
                var validRoles = new List<string> { "Admin", "Manager", "User" };
                Console.WriteLine($"Vai trò hợp lệ: {string.Join(", ", validRoles)}");
                if (!validRoles.Contains(role))
                {
                    return "Vai trò không hợp lệ.";
                }
                var validMaBoPhan = _sqlConnectionServer.BoPhans.Select(bp => bp.MaBP).ToList();
                Console.WriteLine($"Danh sách mã bộ phận: {string.Join(", ", validMaBoPhan)}");
                var validMaChucVu = _sqlConnectionServer.ChucVus.Select(cv => cv.MaChucVu).ToList();
                if (!validMaBoPhan.Contains(nhanvien.MaBP))
                {
                    return "Mã bộ phận không hợp lệ.";
                }
                var validChucVuForBoPhan = _sqlConnectionServer.ChucVus.Where(cv => cv.MaBP == nhanvien.MaBP)
                    .Select(cv => cv.MaChucVu).ToList();
                if (!validChucVuForBoPhan.Contains(nhanvien.MaChucVu))
                {
                    return "Mã chức vụ không phù hợp với mã bộ phận.";
                }
                nhanvien.MaNV = GenerateMaNhanVien(sothutu);
                nhanvien.TenNV = nhanvien.TenNV;
                nhanvien.UserName = $"{nhanvien.TenNV.Replace(" ", " ")}_{nhanvien.MaNV}";
                nhanvien.Passwords = GenerateRandomPassword();
                nhanvien.PasswordHash = HashPassword(nhanvien.Passwords);
                nhanvien.TrangThaiTaiKhoan = true;
                nhanvien.NgayTao = DateTime.Now;
                nhanvien.DangNhapLanDau = true;
                nhanvien.GioiTinh = nhanvien.GioiTinh;
                nhanvien.NgayVaoLam = DateTime.Now;
                nhanvien.Locked = false;
                nhanvien.Roles = role;
                nhanvien.Email = nhanvien.Email;
                nhanvien.DiaChi = nhanvien.DiaChi;
                nhanvien.SDT = nhanvien.SDT;
                nhanvien.NgaySinh = nhanvien.NgaySinh;
                nhanvien.MaChucVu = nhanvien.MaChucVu;
                nhanvien.MaBP = nhanvien.MaBP;
                nhanvien.CCCD = nhanvien.CCCD;
                nhanvien.Avatar = nhanvien.Avatar;
                
                
                _sqlConnectionServer.NhanViens.Add(nhanvien);
                _sqlConnectionServer.SaveChanges();
                
                string subject = $"Duyệt nhân viên mới: {nhanvien.TenNV}";
                string body = $@"
    <html>
    <body>
        <p>Kính gửi Quản lý,</p>
        <p>Nhân viên {nhanvien.TenNV} với mã nhân viên {nhanvien.MaNV} đã được thêm vào hệ thống. Xin vui lòng duyệt thông tin của họ.</p>
        
        <p>
            <a href='https://your-website.com/duyet/{nhanvien.MaNV}?trangthai=duyet' 
               style='background-color: #4CAF50; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px;'>Duyệt nhân viên</a>
        </p>

        <p>
            <a href='https://your-website.com/duyet/{nhanvien.MaNV}?trangthai=koduyet' 
               style='background-color: #f44336; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px;'>Không duyệt</a>
        </p>

        <p>Trân trọng.</p>
    </body>
    </html>";


                string managerEmail = GetManagerEmail(maQL);
                if (string.IsNullOrEmpty(managerEmail))
                {
                    return "Quản lý không hợp lệ hoặc không tìm thấy email quản lý.";
                }
                SendEmailToManager.IsSendEmail(managerEmail, subject, body);
                if (status == "duyet")
                {
                    subject = "Chúc mừng bạn đã được thêm vào hệ thống đã được quản lý duyệt";
                    body = $@"
            <html>
            <body>
                <p>Chào {nhanvien.TenNV},</p>
                <p>Yêu cầu cấp  tài khoản của bạn đã được quản lý phê duyệt. Vui lòng sử dụng mật khẩu mặc định để đăng nhập lần đầu.</p>
                <p>Mật khẩu mặc định của bạn là: <b>{nhanvien.Passwords}</b></p>
                <p>Hãy đổi mật khẩu ngay sau khi đăng nhập để bảo mật tài khoản.</p>
                <p>Trân trọng.</p>
            </body>
            </html>";
                }
                else if (status == "koduyet")
                {
                    subject = "Yêu cầu cấp tài khoản đã bị quản lý từ chối";
                    body = $@"
            <html>
            <body>
                <p>Chào {nhanvien.TenNV},</p>
                <p>Rất tiếc, yêu cầu reset tài khoản của bạn đã bị quản lý từ chối.</p>
                <p>Nếu bạn cần hỗ trợ thêm, vui lòng liên hệ quản lý của bạn.</p>
                <p>Trân trọng.</p>
            </body>
            </html>";
                }
                ApproveStaff(nhanvien.MaNV, status, subject, body);
                

            

                return "Nhân viên đã được thêm vào hệ thống và email đã được gửi tới quản lý để duyệt.";

            }
            catch (Exception ex)
            {
                return $"Lỗi: {ex.Message}";
            }
        }
        public NhanVien SearchEmployeeByMaNhanVien(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return null;
            }
            var nhanvien = _sqlConnectionServer.NhanViens.SingleOrDefault(u => u.MaNV == searchTerm);
            return nhanvien;
        }
        public string UpdateNhanVien(NhanVien updatenhanVien, string maQL, string status)

        {
            try
            {
                var existingNhanVien = _sqlConnectionServer.NhanViens.FirstOrDefault(nv => nv.MaNV == updatenhanVien.MaNV);
                if (existingNhanVien == null)
                {
                    return "Không tìm thấy nhân viên với mã nhân viên đã cung cấp.";
                }
                if (!CheckPhoneNumber.IsPhoneNumberValid(updatenhanVien.SDT))
                {
                    return "Số điện thoại không hợp lệ.";
                }
                if (!CheckDateOfBirth.IsDateOfBirth(updatenhanVien.NgaySinh))
                {
                    return "Ngày sinh không hợp lệ. Nhân viên phải trên 18 tuổi và không phải ngày trong tương lai.";
                }
                if (!CheckAddress.IsAddress(updatenhanVien.DiaChi))
                {
                    return "Địa chỉ không hợp lệ. Địa chỉ phải có ít nhất 5 ký tự.";
                }
                if (!CheckEmail.IsEmail(updatenhanVien.Email))
                {
                    return "Email không hợp lệ.";
                }
                if (!CheckCitizenIdentification.CheckCCCD(existingNhanVien.CCCD))
                {
                    return "Căn cước công dân không hợp lệ";
                }
                var validRoles = new List<string> { "Admin", "Manager", "User" };
                if (!validRoles.Contains(updatenhanVien.Roles))
                {
                    return "Vai trò không hợp lệ.";
                }
                existingNhanVien.TenNV =  updatenhanVien.TenNV;
                existingNhanVien.DiaChi = updatenhanVien.DiaChi;
                existingNhanVien.SDT = updatenhanVien.SDT;
                existingNhanVien.Email = updatenhanVien.Email;
                existingNhanVien.Roles = updatenhanVien.Roles;
                existingNhanVien.GioiTinh = updatenhanVien.GioiTinh;
                existingNhanVien.NgaySinh = updatenhanVien.NgaySinh;
                existingNhanVien.MaQL = updatenhanVien.MaQL;
                existingNhanVien.MaBP = updatenhanVien.MaBP;
                existingNhanVien.MaChucVu = updatenhanVien.MaChucVu;
                existingNhanVien.NgayCapNhat = DateTime.Now;
                existingNhanVien.Roles = updatenhanVien.Roles;
                existingNhanVien.CCCD = updatenhanVien.CCCD;
                _sqlConnectionServer.SaveChanges();

                
                string subject = $"Duyệt cập nhật thông tin nhân viên : {updatenhanVien.TenNV}";
                string body = $@"
    <html>
    <body>
        <p>Kính gửi Quản lý,</p>
        <p>Nhân viên {updatenhanVien.TenNV} với mã nhân viên {updatenhanVien.MaNV} đã được cập nhật thông tin  vào hệ thống. Xin vui lòng duyệt thông tin của họ.</p>
        
        <p>
            <a href='https://your-website.com/duyet/{updatenhanVien.MaNV}?trangthai=duyet' 
               style='background-color: #4CAF50; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px;'>Duyệt nhân viên</a>
        </p>

        <p>
            <a href='https://your-website.com/duyet/{updatenhanVien.MaNV}?trangthai=koduyet' 
               style='background-color: #f44336; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px;'>Không duyệt</a>
        </p>

        <p>Trân trọng.</p>
    </body>
    </html>";
                string managerEmail = GetManagerEmail(maQL);
                if (string.IsNullOrEmpty(managerEmail))
                {
                    return "Quản lý không hợp lệ hoặc không tìm thấy email quản lý.";
                }
                SendEmailToManager.IsSendEmail(managerEmail, subject, body);
                if (status == "duyet")
                {
                    existingNhanVien.DangNhapLanDau = true;

                    subject = "Thông tin cạp nhật thông tin nhân viên";
                    body = $@"
            <html>
            <body>
                <p>Chào {existingNhanVien.TenNV},</p>
                <p>Thực hiệncập nhật thông tin tài khoản của bạn đã được duyệt.</p>
                <p>Trân trọng.</p>
            </body>
            </html>";
                }
                else if (status == "koduyet")
                {
                    subject ="Thông tin cập nhật tài khoản của bạn đã bị quản lý từ chối";
                    body = $@"
            <html>
            <body>
                <p>Chào {existingNhanVien.TenNV},</p>
                
                <p>Nếu bạn cần hỗ trợ thêm, vui lòng liên hệ quản lý của bạn.</p>
                <p>Trân trọng.</p>
            </body>
            </html>";
                }
                ApproveStaff(existingNhanVien.MaNV, status, subject, body);

                return "Cập nhật thông tin nhân viên thành công.";
            }
            catch (Exception ex)
            {
                return $"Đã xảy ra lỗi khi cập nhật thông tin: {ex.Message}";
            }
        }
        public string DeleteNhanVien(string MaNV, string maQL)
        {
            try
            {
                var deleteNV = _sqlConnectionServer.NhanViens.FirstOrDefault(nv => nv.MaNV == MaNV);
                if(deleteNV == null)
                {
                    return "Nhân viên không tồn tại.";
                }
                var result = _sqlConnectionServer.Database.ExecuteSqlCommand("EXEC DeleteNhanVien @MaNV = {0}", MaNV);


                if (result > 0)
                {


                    string managerEmail = GetManagerEmail(maQL);
                    if (string.IsNullOrEmpty(managerEmail))
                    {
                        return "Quản lý không hợp lệ hoặc không tìm thấy email quản lý.";
                    }
                    string subject = $"Duyệt xoá nhân viên trong hệ thống : {deleteNV.TenNV}";
                    string body = $@"
    <html>
    <body>
        <p>Kính gửi Quản lý,</p>
        <p>Nhân viên {deleteNV.TenNV} với mã nhân viên {deleteNV.MaNV} đã nghỉ việc theo  quy định phải xóa trong hệ thống  . Xin vui lòng duyệt thông tin của họ.</p>
        
        <p>
            <a href='https://your-website.com/duyet/{deleteNV.MaNV}?trangthai=duyet' 
               style='background-color: #4CAF50; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px;'>Duyệt nhân viên</a>
        </p>

        <p>
            <a href='https://your-website.com/duyet/{deleteNV.MaNV}?trangthai=koduyet' 
               style='background-color: #f44336; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px;'>Không duyệt</a>
        </p>

        <p>Trân trọng.</p>
    </body>
    </html>";
                    SendEmailToManager.IsSendEmail(managerEmail, subject, body);
                    return "Nhân viên đã được xóa thành công.";
                }
                else
                {
                    return "Có lỗi xảy ra khi xóa nhân viên.";
                }

            }
            catch(Exception ex)
            {
                return $"Lỗi khi xóa nhân viên: {ex.Message}";
            }
        }
        public string UnLocked(string MaNV, string maQL, string status)
        {
            try
            {
                var unlockedNV = _sqlConnectionServer.NhanViens.FirstOrDefault(u => u.MaNV == MaNV);
                if(unlockedNV == null)
                {
                    return "Nhân viên không tồn tại.";
                }
                if (!unlockedNV.Locked)
                {
                    return "Tài khoản nhân viên đã được mở khóa trước đó.";
                }

                unlockedNV.Locked = false;
                _sqlConnectionServer.SaveChanges();
                
                string subject = $"Duyệt mở khóa  : {unlockedNV.TenNV}";
                string body = $@"
    <html>
    <body>
        <p>Kính gửi Quản lý,</p>
        <p>Nhân viên {unlockedNV.TenNV} với mã nhân viên {unlockedNV.MaNV} đã gửi yêu cầu mở khóa   . Xin vui lòng duyệt thông tin của họ.</p>
        
        <p>
            <a href='https://your-website.com/duyet/{unlockedNV.MaNV}?trangthai=duyet' 
               style='background-color: #4CAF50; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px;'>Duyệt nhân viên</a>
        </p>

        <p>
            <a href='https://your-website.com/duyet/{unlockedNV.MaNV}?trangthai=koduyet' 
               style='background-color: #f44336; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px;'>Không duyệt</a>
        </p>

        <p>Trân trọng.</p>
    </body>
    </html>";
                
                string managerEmail = GetManagerEmail(maQL);
                if (string.IsNullOrEmpty(managerEmail))
                {
                    return "Quản lý không hợp lệ hoặc không tìm thấy email quản lý.";
                }
                SendEmailToManager.IsSendEmail(managerEmail, subject, body);
                if (status == "duyet")
                {
                    unlockedNV.DangNhapLanDau = true;
                    
                    subject = "Thông tin mở khóa tài khoản  ";
                    body = $@"
            <html>
            <body>
                <p>Chào {unlockedNV.TenNV},</p>
                <p>Thực hiện yêu cầu mở khóa tài khoản của bạn đã được duyệt  .</p>
                <p>Trân trọng.</p>
            </body>
            </html>";
                }
                else if (status == "koduyet")
                {
                    subject ="Yêu cầu mở khóa tài khoản đã bị từ chối do quản lý";
                    body = $@"
            <html>
            <body>
                <p>Chào {unlockedNV.TenNV},</p>
                
                <p>Nếu bạn cần hỗ trợ thêm, vui lòng liên hệ quản lý của bạn.</p>
                <p>Trân trọng.</p>
            </body>
            </html>";
                }
                ApproveStaff(unlockedNV.MaNV, status, subject, body);


                return "Tài khoản nhân viên đã được mở khóa thành công.";
            }
            catch(Exception ex)
            {
                return $"Lỗi khi mở khóa tài khoản: {ex.Message}";
            }
        }
        public string Locked(string MaNV, string maQL,string status, string reason = "Vi phạm chính sách")
        {
            try
            {
                var Lock = _sqlConnectionServer.NhanViens.FirstOrDefault(u => u.MaNV == MaNV);
                if(Lock == null)
                {
                    return "Nhân viên không tồn tại.";
                }
                if (Lock.Locked)
                {
                    return "Tài khoản nhân viên đã bị khóa trước đó.";
                }
                Lock.Locked = true;
                Lock.LyDo = reason;
                _sqlConnectionServer.SaveChanges();
               
                string subject = $"Duyệt Khóa User Nhân Viên Trong Hệ Thống   : {Lock.TenNV}";
                string body = $@"
    <html>
    <body>
        <p>Kính gửi Quản lý,</p>
        <p>Nhân viên {Lock.TenNV} với mã nhân viên {Lock.MaNV} đã bị khóa trong hệ thống Lý do:{Lock.LyDo}  . Xin vui lòng duyệt thông tin của họ.</p>
        
        <p>
            <a href='https://your-website.com/duyet/{Lock.MaNV}?trangthai=duyet' 
               style='background-color: #4CAF50; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px;'>Duyệt nhân viên</a>
        </p>

        <p>
            <a href='https://your-website.com/duyet/{Lock.MaNV}?trangthai=koduyet' 
               style='background-color: #f44336; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px;'>Không duyệt</a>
        </p>

        <p>Trân trọng.</p>
    </body>
    </html>";

                string managerEmail = GetManagerEmail(maQL);
                if (string.IsNullOrEmpty(managerEmail))
                {
                    return "Quản lý không hợp lệ hoặc không tìm thấy email quản lý.";
                }
                SendEmailToManager.IsSendEmail(managerEmail, subject, body);
                if (status == "duyet")
                {
                    subject = "Thông tin tài khoản bị khóa ";
                    body = $@"
            <html>
            <body>
                <p>Chào {Lock.TenNV},</p>
                <p>Do tài khoản vi phạm chính sách .Thông tin đến bạn mọi thứac mắc vui liên hệ phòng IT .</p>
               
                
                <p>Trân trọng.</p>
            </body>
            </html>";
                }
                else if (status == "koduyet")
                {
                    subject = "Yêu cầu reset tài khoản của bạn đã bị từ chối";
                    body = $@"
            <html>
            <body>
                <p>Chào {Lock.TenNV},</p>
                
                <p>Nếu bạn cần hỗ trợ thêm, vui lòng liên hệ quản lý của bạn.</p>
                <p>Trân trọng.</p>
            </body>
            </html>";
                }
                ApproveStaff(Lock.MaNV, status, subject, body);
               


                return "Tài khoản nhân viên đã được khóa thành công.";

            }
            catch(Exception ex)
            {
                return $"Lỗi khi khóa tài khoản: {ex.Message}";
            }
        }
        public NhanVien FindNhanVienByMaNV(string MaNV)
        {
            try
            {
                var nhanvien = _sqlConnectionServer.NhanViens.FirstOrDefault(u => u.MaNV == MaNV);
                if (nhanvien == null)
                {
                    throw new Exception("Nhân viên không tồn tại.");
                }
                return nhanvien;
            }
            catch(Exception ex)
            {
                throw new Exception($"Lỗi khi tìm nhân viên: {ex.Message}");
            }
        }
        public string EmployeeRightsGrant(string MaNV, string MaQuyen)
        {
            try
            {
                var nhanvien = _sqlConnectionServer.NhanViens.FirstOrDefault(nv => nv.MaNV == MaNV);
                if(nhanvien == null)
                {
                    return "Nhân viên không tồn tại.";
                }

                var phanquyen = _sqlConnectionServer.PhanQuyens.FirstOrDefault(pq => pq.MaQuyen == MaQuyen);
                if(phanquyen == null)
                {
                    return "Quyền không tồn tại.";
                }
                nhanvien.PhanQuyen = phanquyen;
                _sqlConnectionServer.SaveChanges();

                return $"Quyền {phanquyen.TenQuyen} đã được cấp cho nhân viên {nhanvien.TenNV}.";
            }
            catch(Exception ex)
            {
                return $"Lỗi khi cấp quyền: {ex.Message}";
            }
        }

        public string GeneratePermissionCode(string TenQuyen)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TenQuyen))
                {
                    return "Tên quyền không hợp lệ.";
                }
                string MaQuyen = string.Join(" ", TenQuyen.Split(' ')
                    .Where(word => !string.IsNullOrWhiteSpace(word))
                    .Select(word => char.ToUpper(word[0])));

                var existingQuyen = _sqlConnectionServer.PhanQuyens.FirstOrDefault(pq => pq.MaQuyen == MaQuyen);
                if(existingQuyen != null)
                {
                    int count = 1;
                    string NewRights;
                    do
                    {
                        NewRights = $"{MaQuyen}{count++}";
                    } while (_sqlConnectionServer.PhanQuyens.Any(pq => pq.MaQuyen == NewRights));
                    MaQuyen = NewRights;
                }
                return MaQuyen;
            }
            catch(Exception ex)
            {
                return $"Lỗi: {ex.Message}";
            }
        }
        public string ResetUser(string username, string maQL, string status, string subject, string body)
        {
            try
            {
                var user = _sqlConnectionServer.NhanViens.FirstOrDefault(u => u.UserName == username);
                if(user == null)
                {
                    return "Không tìm thấy tài khoản với tên đăng nhập này.";
                }
                string defaultPassword = GenerateRandomPassword();
                string hashPassword = HashPassword(defaultPassword);
                user.Passwords = defaultPassword;
                user.PasswordHash = hashPassword;
                user.DangNhapLanDau = true;
                user.Locked = false;
                user.TrangThaiTaiKhoan = true;
                _sqlConnectionServer.SaveChanges();
               
                 subject = $"Duyệt Reset User trong hệ thống    : {user.TenNV}";
                 body = $@"
    <html>
    <body>
        <p>Kính gửi Quản lý,</p>
        <p>Nhân viên {user.TenNV} với mã nhân viên {user.MaNV}  đã gửi yêu cầu reset  . Xin vui lòng duyệt thông tin của họ.</p>
        
        <p>
            <a href='https://your-website.com/duyet/{user.MaNV}?trangthai=duyet' 
               style='background-color: #4CAF50; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px;'>Duyệt nhân viên</a>
        </p>

        <p>
            <a href='https://your-website.com/duyet/{user.MaNV}?trangthai=koduyet' 
               style='background-color: #f44336; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px;'>Không duyệt</a>
        </p>

        <p>Trân trọng.</p>
    </body>
    </html>";
                string managerEmail = GetManagerEmail(maQL);
                if (string.IsNullOrEmpty(managerEmail))
                {
                    return "Quản lý không hợp lệ hoặc không tìm thấy email quản lý.";
                }
                SendEmailToManager.IsSendEmail(managerEmail, subject, body);
                if(status == "duyet")
                {
                    subject = "Yêu cầu tài khoản của bạn đã được duyệt";
                    body = $@"
            <html>
            <body>
                <p>Chào {user.TenNV},</p>
                <p>Yêu cầu reset tài khoản của bạn đã được quản lý phê duyệt. Vui lòng sử dụng mật khẩu mặc định để đăng nhập lần đầu.</p>
                <p>Mật khẩu mặc định của bạn là: <b>{user.Passwords}</b></p>
                <p>Hãy đổi mật khẩu ngay sau khi đăng nhập để bảo mật tài khoản.</p>
                <p>Trân trọng.</p>
            </body>
            </html>";
                }else if(status == "koduyet")
                {
                    subject = "Yêu cầu reset tài khoản của bạn đã bị từ chối";
                    body = $@"
            <html>
            <body>
                <p>Chào {user.TenNV},</p>
                <p>Rất tiếc, yêu cầu reset tài khoản của bạn đã bị quản lý từ chối.</p>
                <p>Nếu bạn cần hỗ trợ thêm, vui lòng liên hệ quản lý của bạn.</p>
                <p>Trân trọng.</p>
            </body>
            </html>";
                }
                ApproveStaff(user.MaNV, status, subject, body);
                return "Đã gửi email yêu cầu duyệt và thông báo cho nhân viên.";

            }

            catch (Exception ex)
            {
                return $"Lỗi: {ex.Message}";
            }
        }
        // Phương thức tạo mã máy Pos(máy tính tiền )
        public string GenerateMaPos(string NamePos, string NumberLocationPos)
        {
            // Lọc ra các ký tự chữ từ tên máy
            var letters = new string(NamePos.Where(c => Char.IsLetter(c)).ToArray());

            // Nếu không có ký tự nào trong tên máy, trả về thông báo lỗi
            if (string.IsNullOrEmpty(letters))
            {
                return "Tên máy không hợp lệ.";
            }
            // Tạo mã MaPos bằng cách kết hợp các ký tự chữ từ tên máy và số quầy
            string maPos = letters.ToUpper() + NumberLocationPos;
            return maPos;

        }
        // Phương thức thêm máy pos (máy tính tiền )
        public string AddPos(MayPos mayPos)
        {
            try
            {
                if (!IsValidIpAddress(mayPos.IpAddress))
                {
                    LockIp(mayPos.IpAddress);
                    return "IP không hợp lệ. Máy POS đã bị khoá.";
                }
                _sqlConnectionServer.MayPos.Add(mayPos);
                _sqlConnectionServer.SaveChanges();

                return "Máy POS đã được thêm thành công.";
            }
            catch(Exception ex)
            {
                return $"Lỗi khi thêm máy POS: {ex.Message}";
            }
        }
        public string UpdatePos(int maPos, string tenMay, string ipAddress, string soQuay, bool trangThai, string maNV)
        {
            try
            {
                var mayPos = _sqlConnectionServer.MayPos.FirstOrDefault(mp => mp.MaPos == maPos);
                if(mayPos == null)
                {
                    return "Máy POS không tồn tại.";
                }
                if (!IsValidIpAddress(mayPos.IpAddress))
                {
                    LockIp(ipAddress);  // Khoá IP không hợp lệ
                    return "IP không hợp lệ. Máy POS đã bị khoá.";
                }
                mayPos.TenMay = tenMay;
                mayPos.IpAddress = ipAddress;
                mayPos.SoQuay = soQuay;
                mayPos.TrangThai = trangThai;
                mayPos.MaNV = maNV;
                _sqlConnectionServer.SaveChanges();

                return "Máy POS đã được cập nhật thành công.";
            }
            catch(Exception ex)
            {
                return $"Lỗi khi cập nhật máy POS: {ex.Message}";
            }
        }
        public void ScanAndLockInvalidIps()
        {
            var allMayPos = _sqlConnectionServer.MayPos.ToList();
            foreach(var mayPos in allMayPos)
            {
                if (!IsValidIpAddress(mayPos.IpAddress))
                {
                    LockIp(mayPos.IpAddress);
                }
            }
        }
        // Khóa máy POS nếu IP không hợp lệ
        private void LockIp(string ipAddress)
        {
            var mayPos = _sqlConnectionServer.MayPos.FirstOrDefault(mp => mp.IpAddress == ipAddress);
            if (mayPos != null)
            {
                mayPos.TrangThai = false; // Đánh dấu là đã bị khoá
                _sqlConnectionServer.SaveChanges();
            }
        }
        // Kiểm tra xem địa chỉ IP có hợp lệ hay không
        private bool IsValidIpAddress(string ipAddress)
        {
            return !string.IsNullOrEmpty(ipAddress) &&
                   System.Net.IPAddress.TryParse(ipAddress, out _); // Kiểm tra định dạng IP hợp lệ
        }
        // Phương thức tạo mã bộ phận 
        public string CreatePartNumber(string TenBP)
        {
            // Lấy 3 ký tự đầu tiên từ tên bộ phận và chuyển thành chữ hoa
            var initials = string.Concat(TenBP.Split(' ').Select(word => word[0].ToString().ToLower()));

            initials = initials.Length > 3 ? initials.Substring(0, 3) : initials;

            var existingDepartments = _sqlConnectionServer.BoPhans.ToList();
            var departmentCount = existingDepartments.Count();

            var MaBP = $"BP_{initials}-{departmentCount + 1:D3}";
            return MaBP;
        }

        // Phương thức thêm bộ phận
        public bool AddParts(string TenBP)
        {
            var PartNumber = CreatePartNumber(TenBP);
            var newParts = new BoPhan
            {
                MaBP = PartNumber,
                TenBoPhan = TenBP
            };
            _sqlConnectionServer.BoPhans.Add(newParts);
            return _sqlConnectionServer.SaveChanges() > 0;
        }
        // Phương thức sửa bộ phận
        public bool UpdateParts(string maBP, string TenBp)
        {
            var Bophan = _sqlConnectionServer.BoPhans.FirstOrDefault(bp => bp.MaBP == maBP);
            if(Bophan != null)
            {
                Bophan.TenBoPhan = TenBp;
                return _sqlConnectionServer.SaveChanges() > 0;
            }
            return false;
        }
        //Phương thức xoa bộ phận 
        public bool DeleteParts(string MaBP)
        {
            var BoPhan = _sqlConnectionServer.BoPhans.FirstOrDefault(bp => bp.MaBP == MaBP);
            if (BoPhan != null)
            {
                _sqlConnectionServer.BoPhans.Remove(BoPhan);
                 return _sqlConnectionServer.SaveChanges() > 0;
            }
            return false;
        }
        // Phương thức tạo mã chức vụ 
        public string CreatePositionCode(string TenCV)
        {
            string maCV = TenCV.Substring(0, Math.Min(3, TenCV.Length));
            maCV = maCV.ToLower().Replace(" ", "");
            return maCV;
        }
        // Phương thức thêm chức vụ 
        public bool AddPosition(string TenCV)
        {
            var MaCV = CreatePositionCode(TenCV);
            var newPostion = new ChucVu
            {
                MaChucVu = MaCV,
                TenChucVu = TenCV
            };
            _sqlConnectionServer.ChucVus.Add(newPostion);
            return _sqlConnectionServer.SaveChanges() > 0;
        }
        // Phương thức cập nhật thông tin chức vụ 
        public bool UpdatePosition(string maCV, string TenCV)
        {
            var chucvu = _sqlConnectionServer.ChucVus.FirstOrDefault(u => u.MaChucVu == maCV);
            if (chucvu != null)
            {
                chucvu.TenChucVu = TenCV;
                _sqlConnectionServer.SaveChanges();
                return true;
            }
            return false;
        } 
        //Phương thức xóa chức vụ 
        public bool DeletePosition(string maCV)
        {
            var chucvu = _sqlConnectionServer.ChucVus.FirstOrDefault(u => u.MaChucVu == maCV);
            if(chucvu != null)
            {
                _sqlConnectionServer.ChucVus.Remove(chucvu);
                _sqlConnectionServer.SaveChanges();
                return true;
            }
            return false;
        }
        public bool LogOutAsync()
        {
            try
            {
                // Lấy tên người dùng hiện tại từ Authentication
                var userName = HttpContext.Current.User.Identity.Name;

                // Tìm nhân viên trong cơ sở dữ liệu theo tên đăng nhập
                var nhanVien = _sqlConnectionServer.NhanViens.FirstOrDefault(nv => nv.UserName == userName);

                if (nhanVien != null)
                {
                    // Cập nhật thời gian đăng xuất cho nhân viên
                    nhanVien.ThoiGianDangXuat = DateTime.Now;
                    _sqlConnectionServer.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu
                }

                // Đăng xuất người dùng bằng cách gọi phương thức SignOut
                FormsAuthentication.SignOut();

                return true;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return false;
            }
        }

        // Phương thức mã hóa mật khẩu 
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        // Phương thức truy vấn Email có chức vụ quản lý 
        public string GetManagerEmail(string maQL)
        {
            var manager = _sqlConnectionServer.QuanLies.FirstOrDefault(u => u.MaQL == maQL);
            return manager?.Email;
        }
        public void ApproveStaff(string maNV, string status, string subject, string body)
        {
            var nhanvien = _sqlConnectionServer.NhanViens.FirstOrDefault(nv => nv.MaNV == maNV);
            if(nhanvien != null)
            {
                

                if (status == "duyet")
                {
                    nhanvien.TrangThaiDuyet = true;
                   
                    
                }else if(status == "koduyet")
                {
                    nhanvien.TrangThaiDuyet = false;
                   
                }
                _sqlConnectionServer.SaveChanges();
                if (string.IsNullOrEmpty(nhanvien.Email))
                {
                    SendEmailToManager.IsSendEmail(nhanvien.Email, subject, body);
                }
            }
           
        }

    }
}
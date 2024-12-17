
// Custom JavaScript for form validation or other actions
document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");
    form.addEventListener("submit", function (event) {
        let isValid = true;
        // You can add more validation here if needed
        if (document.getElementById("HoTen").value === "") {
            isValid = false;
            alert("Họ tên không được để trống.");
        }

        if (!isValid) {
            event.preventDefault(); // Prevent form submission
        }
    });
});
$(document).ready(function () {
    // Xử lý sự kiện submit của form thêm nhân viên
    $("#addEmployeeForm").submit(function (event) {
        event.preventDefault(); // Ngừng gửi form theo cách mặc định

        var formData = $(this).serialize(); // Lấy dữ liệu từ form

        // Gửi dữ liệu bằng AJAX
        $.ajax({
            url: '/Admin/AddEmployee', // URL xử lý request
            type: 'POST',
            data: formData,
            success: function (response) {
                // Kiểm tra nếu thêm nhân viên thành công
                if (response.success) {
                    // Thêm một dòng mới vào bảng
                    var newRow = "<tr>" +
                        "<td>" + response.nhanVien.maBoPhan + "</td>" +
                        "<td>" + response.nhanVien.MaChucVu + "</td>" +
                        "<td>" + response.nhanVien.HoTen + "</td>" +
                        "<td>" + response.nhanVien.CCCD + "</td>" + 
                        "<td>" + response.nhanVien.GioiTinh + "</td>" +
                        "<td>" + response.nhanVien.NgaySinh + "</td>" +
                        "<td>" + response.nhanVien.DiaChi + "</td>" +
                        "<td>" + response.nhanVien.SDT + "</td>" +
                        "<td>" + response.nhanVien.Email + "</td>" +
                        "<td>" + response.nhanVien.Roles + "</td>" +
                        "<td>" + response.nhanVien.UserName + "</td>" + 
                        "<td>" + response.nhanVien.TrangThaiTaiKhoan + "</td>" +
                        "<td>" + response.nhanVien.NgayTao + "</td>" +
                        "</tr>";
                    $("#employeeTable tbody").append(newRow); // Thêm vào cuối bảng
                } else {
                    alert("Có lỗi khi thêm nhân viên!");
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi trong quá trình gửi dữ liệu!");
            }
        });
    });
});




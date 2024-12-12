document.getElementById("MaBoPhan").addEventListener("change", function () {
    var maBoPhan = this.value;
    fetch(`/GetChucVuByBoPhan?maBoPhan=${maBoPhan}`)
        .then(response => response.json())
        .then(data => {
            var chucVuDropdown = document.getElementById("MaChucVu");
            chucVuDropdown.innerHTML = "";
            data.forEach(chucVu => {
                var option = document.createElement("option");
                option.value = chucVu.MaChucVu;
                option.textContent = chucVu.TenChucVu;
                chucVuDropdown.appendChild(option);
            });
        });
});
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


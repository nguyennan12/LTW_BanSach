# 📘 TÀI LIỆU CHI TIẾT: FLOW ĐẶT HÀNG SÁCH (BOOK ORDER FLOW)
**Dự án:** Web Bán Sách "Nhà Giả Kim" (ASP.NET Core 8.0 MVC)  
**Kiến trúc:** Clean 3-Tier Architecture (View $\rightarrow$ Controller $\rightarrow$ Service $\rightarrow$ DbContext $\rightarrow$ SQLite)

---

## 🧭 TỔNG QUAN LUỒNG ĐẶT HÀNG (END-TO-END)

```
[Khách Hàng Điền Form & Submit]
               │
               ▼ (HTTP POST /Home/DatHang)
[HomeController] 
   ├── 1. Kiểm tra Annotation dữ liệu (ModelState.IsValid)
   └── 2. Gọi IDonHangService.TaoDonHangAsync(datHangForm)
               │
               ▼ (Gọi nghiệp vụ Business Logic)
[DonHangService]
   ├── 3. Kiểm tra thông tin sách & Số lượng tồn kho (SoLuongTon)
   ├── 4. Trừ số lượng tồn kho (SoLuongTon -= SoLuong)
   ├── 5. Tạo thực thể DonHang (TrangThai = 'Chờ xử lý')
   └── 6. Lưu vào Database qua AppDbContext.SaveChangesAsync()
               │
               ▼ (Trả kết quả về Controller)
[HomeController]
   ├── Thành công: RedirectToAction("CamOn", new { id = donHang.Id })
   └── Thất bại: Trả về View("Index") kèm thông báo lỗi đỏ
               │
               ▼
[Trang Cảm Ơn / Hóa Đơn (CamOn.cshtml)]
```

---

## 🔍 CHI TIẾT TỪNG BƯỚC XỬ LÝ TRONG CODE

### Bước 1: Giao diện Form Đặt Hàng (Client / View)
* **File:** [Views/Home/Index.cshtml](file:///E:/BanSachMvc/Views/Home/Index.cshtml) (vùng `#dat-hang`)
* **Đoạn code xử lý:** Form Razor gửi dữ liệu `POST` sang action `DatHang` của `HomeController`:
```html
<form asp-controller="Home" asp-action="DatHang" method="post" id="orderForm">
    @Html.AntiForgeryToken()
    
    <!-- Họ tên -->
    <input asp-for="DatHangForm.HoTen" class="form-control" placeholder="Họ và tên..." />
    <span asp-validation-for="DatHangForm.HoTen" class="text-danger"></span>

    <!-- Số điện thoại -->
    <input asp-for="DatHangForm.SoDienThoai" class="form-control" placeholder="0987654321" />
    <span asp-validation-for="DatHangForm.SoDienThoai" class="text-danger"></span>

    <!-- Địa chỉ nhận hàng -->
    <input asp-for="DatHangForm.DiaChi" class="form-control" placeholder="Địa chỉ giao hàng..." />
    <span asp-validation-for="DatHangForm.DiaChi" class="text-danger"></span>

    <!-- Số lượng (JS tính tiền tự động) -->
    <input asp-for="DatHangForm.SoLuong" id="inputSoLuong" type="number" min="1" max="20" />

    <!-- Phương thức thanh toán -->
    <input type="radio" asp-for="DatHangForm.PhuongThucThanhToan" value="COD" checked /> COD
    <input type="radio" asp-for="DatHangForm.PhuongThucThanhToan" value="ChuyenKhoan" /> Chuyển khoản

    <!-- Nút Submit -->
    <button type="submit" class="btn btn-primary-custom">XÁC NHẬN ĐẶT HÀNG NGAY</button>
</form>
```

---

### Bước 2: Controller Tiếp Nhận Request
* **File:** [Controllers/HomeController.cs](file:///E:/BanSachMvc/Controllers/HomeController.cs)
* **Action:** `[HttpPost] public async Task<IActionResult> DatHang(DatHangViewModel datHangForm)`
* **Nhiệm vụ:**
  1. Kiểm tra validation cơ bản (`ModelState.IsValid`: số điện thoại 10 chữ số, họ tên, địa chỉ không rỗng).
  2. Không trực tiếp xử lý database, mà chuyển toàn bộ dữ liệu xuống tầng Service `_donHangService.TaoDonHangAsync(...)`.
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DatHang(DatHangViewModel datHangForm)
{
    var sach = await _sachService.LaySachChinhAsync();
    if (sach == null) return NotFound();

    datHangForm.Sach = sach;

    // 1. Nếu form nhập sai định dạng (ví dụ SĐT sai cú pháp)
    if (!ModelState.IsValid)
    {
        var feedbacks = await _feedbackService.LayFeedbackDaDuyetAsync(sach.Id);
        var vm = new LandingPageViewModel { Sach = sach, Feedbacks = feedbacks, DatHangForm = datHangForm };
        return View("Index", vm);
    }

    // 2. Gọi Service xử lý đặt hàng
    var (thanhCong, loi, donHang) = await _donHangService.TaoDonHangAsync(datHangForm);
    
    // 3. Nếu Service báo lỗi nghiệp vụ (ví dụ hết hàng)
    if (!thanhCong || donHang == null)
    {
        ModelState.AddModelError(string.Empty, loi ?? "Đã xảy ra lỗi khi tạo đơn hàng.");
        var feedbacks = await _feedbackService.LayFeedbackDaDuyetAsync(sach.Id);
        var vm = new LandingPageViewModel { Sach = sach, Feedbacks = feedbacks, DatHangForm = datHangForm };
        return View("Index", vm);
    }

    // 4. Đặt thành công -> Điều hướng sang trang Cảm ơn
    return RedirectToAction(nameof(CamOn), new { id = donHang.Id });
}
```

---

### Bước 3: Tầng Service Xử Lý Nghiệp Vụ (Business Logic)
* **File Interface:** [Services/IDonHangService.cs](file:///E:/BanSachMvc/Services/IDonHangService.cs)
* **File Implementation:** [Services/DonHangService.cs](file:///E:/BanSachMvc/Services/DonHangService.cs)
* **Nhiệm vụ:**
  1. Kiểm tra sách có tồn tại trong hệ thống hay không.
  2. Kiểm tra số lượng đặt mua so với số lượng còn lại trong kho (`model.SoLuong > sach.SoLuongTon`).
  3. Trừ số lượng tồn kho của sách (`sach.SoLuongTon -= model.SoLuong`).
  4. Tính tổng tiền: `TongTien = sach.Gia * model.SoLuong`.
  5. Thêm thực thể `DonHang` mới vào database và gọi `_db.SaveChangesAsync()`.
```csharp
public async Task<(bool ThanhCong, string? ThongBaoLoi, DonHang? DonHang)> TaoDonHangAsync(DatHangViewModel model)
{
    var sach = await _db.Sach.FirstOrDefaultAsync();
    if (sach == null)
    {
        return (false, "Không tìm thấy thông tin sản phẩm sách.", null);
    }

    if (model.SoLuong <= 0)
    {
        return (false, "Số lượng đặt mua phải lớn hơn 0.", null);
    }

    // Kiểm tra tồn kho
    if (model.SoLuong > sach.SoLuongTon)
    {
        return (false, $"Số lượng trong kho chỉ còn {sach.SoLuongTon} cuốn, không đủ để giao.", null);
    }

    // Tạo đơn hàng mới
    var donHang = new DonHang
    {
        SachId = sach.Id,
        HoTen = model.HoTen.Trim(),
        SoDienThoai = model.SoDienThoai.Trim(),
        DiaChi = model.DiaChi.Trim(),
        PhuongThucThanhToan = model.PhuongThucThanhToan,
        GhiChu = model.GhiChu?.Trim(),
        SoLuong = model.SoLuong,
        TongTien = sach.Gia * model.SoLuong,
        NgayDat = DateTime.Now,
        TrangThai = "Chờ xử lý"
    };

    // Giảm số lượng tồn kho của cuốn sách
    sach.SoLuongTon -= model.SoLuong;
    
    // Lưu vào cơ sở dữ liệu SQLite
    _db.DonHang.Add(donHang);
    await _db.SaveChangesAsync();

    return (true, null, donHang);
}
```

---

### Bước 4: Hiển Thị Trang Cảm Ơn / Hóa Đơn (Thank You Page)
* **Controller:** Action [`HomeController.CamOn(int id)`](file:///E:/BanSachMvc/Controllers/HomeController.cs)
```csharp
[HttpGet]
public async Task<IActionResult> CamOn(int id)
{
    var don = await _donHangService.LayDonHangTheoIdAsync(id);
    if (don == null) return NotFound();
    return View(don);
}
```
* **View:** [Views/Home/CamOn.cshtml](file:///E:/BanSachMvc/Views/Home/CamOn.cshtml) nhận model `DonHang` và hiển thị:
  * Mã đơn hàng: `#@Model.Id`
  * Thông tin khách hàng & địa chỉ giao hàng
  * Sản phẩm, số lượng, tổng tiền thanh toán
  * Hướng dẫn thanh toán (nếu chọn chuyển khoản) và nút In hóa đơn.

---

# BanSach – Landing Page Bán Sách "Nhà Giả Kim" (ASP.NET Core 8.0 MVC)

Website bán sách trực tuyến theo mô hình chuẩn **3-Tier Clean Architecture (View $\rightarrow$ Controller $\rightarrow$ Service $\rightarrow$ DbContext $\rightarrow$ SQLite)** với giao diện hiện đại, tối ưu trải nghiệm đặt hàng cho độc giả.

---

## 🎥 Video Demo Hoạt Động
* **Link xem video demo trực tiếp:** [Google Drive Demo Video](https://drive.google.com/file/d/1XTNxmDQ81E89QU0wkf3fr9kuv8cz100P/view?usp=sharing)

---

## 🚀 Hướng Dẫn Chạy Dự Án (Getting Started)

### 1. Yêu cầu môi trường
* Đã cài đặt [.NET 8 SDK](https://dotnet.microsoft.com/download)

### 2. Các bước khởi chạy
Mở Terminal trong thư mục dự án và chạy lệnh:

```bash
# Khôi phục dependencies và chạy ứng dụng
dotnet run --urls="http://localhost:5000"
```

Mở trình duyệt truy cập: 👉 **[http://localhost:5000](http://localhost:5000)**

---

## 🔄 Luồng Xử Lý Đặt Hàng (Order Flow)
1. **Khách hàng điền form:** Chọn số lượng, họ tên, số điện thoại, địa chỉ và phương thức thanh toán tại `Index.cshtml`.
2. **Controller tiếp nhận:** `HomeController.DatHang` kiểm tra validation hợp lệ.
3. **Service xử lý:** `DonHangService.TaoDonHangAsync` kiểm tra tồn kho, trừ số lượng tồn và lưu đơn hàng vào SQLite.
4. **Trả kết quả:** Chuyển hướng sang trang hóa đơn `Home/CamOn/{id}`.

*(Chi tiết xem thêm tại file [FLOW_XU_LY.md](FLOW_XU_LY.md))*

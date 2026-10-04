# BanSach – web bán MỘT cuốn sách (ASP.NET Core 8 MVC)

## Chạy
1. Cài .NET 8 SDK: https://dotnet.microsoft.com/download
2. Mở terminal trong thư mục dự án:
       dotnet run
3. Mở địa chỉ hiện ra (vd http://localhost:5000).

## Sửa thông tin sách
Mở `Data/AppDbContext.cs`, sửa tên, tác giả, mô tả, giá, số lượng.
Đổi ảnh bìa: thay file `wwwroot/img/bia.svg`.
Làm TRƯỚC khi chạy lần đầu. Nếu đã chạy rồi, xóa file `bansach.db` rồi chạy lại.

## Xem đơn hàng
Vào: /QuanTri?key=MẬT_KHẨU
Mật khẩu đặt ở `appsettings.json` (mục QuanTri > MatKhau) — nhớ đổi.

## Cấu trúc MVC
- Models/        (M) : Sach, DonHang, DatHangViewModel
- Views/         (V) : Home/Index, Home/CamOn, QuanTri/Index, Shared/_Layout
- Controllers/   (C) : HomeController (đặt hàng), QuanTriController (xem đơn)
- Data/AppDbContext  : kết nối database (SQLite, file bansach.db)

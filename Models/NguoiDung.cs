namespace BanSach.Models;

public class NguoiDung
{
    public int Id { get; set; }
    public string TenDangNhap { get; set; } = "";
    public string MatKhau { get; set; } = "";
    public string HoTen { get; set; } = "";
    public string SoDienThoai { get; set; } = "";
    public string DiaChi { get; set; } = "";
    public string VaiTro { get; set; } = "User"; // "Admin" hoặc "User"
    public DateTime NgayTao { get; set; } = DateTime.Now;

    // Navigation property
    public List<DonHang> DanhSachDonHang { get; set; } = new();
}

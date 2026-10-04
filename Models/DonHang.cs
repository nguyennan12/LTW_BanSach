namespace BanSach.Models;

public class DonHang
{
    public int Id { get; set; }
    public int SachId { get; set; }
    public Sach? Sach { get; set; }

    public string HoTen { get; set; } = "";
    public string SoDienThoai { get; set; } = "";
    public string DiaChi { get; set; } = "";
    public string PhuongThucThanhToan { get; set; } = "COD"; // COD, ChuyenKhoan
    public string? GhiChu { get; set; }
    public int SoLuong { get; set; }
    public long TongTien { get; set; }
    public DateTime NgayDat { get; set; } = DateTime.Now;
    public string TrangThai { get; set; } = "Chờ xử lý";   // Chờ xử lý | Đang giao | Đã giao | Đã hủy
}

using System.ComponentModel.DataAnnotations;
using BanSach.Models;

namespace BanSach.ViewModels;

public class DatHangViewModel
{
    public Sach? Sach { get; set; }

    public int? NguoiDungId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ và tên của bạn")]
    [StringLength(100, ErrorMessage = "Họ tên không vượt quá 100 ký tự")]
    public string HoTen { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Số điện thoại không hợp lệ (10 chữ số, bắt đầu bằng 03, 05, 07, 08, 09)")]
    public string SoDienThoai { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng chi tiết")]
    [StringLength(300, ErrorMessage = "Địa chỉ không vượt quá 300 ký tự")]
    public string DiaChi { get; set; } = "";

    [Range(1, 20, ErrorMessage = "Số lượng đặt mua từ 1 đến 20 cuốn")]
    public int SoLuong { get; set; } = 1;

    public string PhuongThucThanhToan { get; set; } = "COD"; // COD hoặc ChuyenKhoan

    public string? GhiChu { get; set; }
}

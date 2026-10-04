using BanSach.Models;

namespace BanSach.Services;

public interface IDonHangService
{
    Task<(bool ThanhCong, string? ThongBaoLoi, DonHang? DonHang)> TaoDonHangAsync(DatHangViewModel model);
    Task<DonHang?> LayDonHangTheoIdAsync(int id);
    Task<List<DonHang>> LayDanhSachDonHangAsync(string? trangThai = null, string? timKiem = null);
    Task<bool> CapNhatTrangThaiAsync(int id, string trangThaiMoi);
    Task<(int TongDon, int DonMoi, int DaGiao, long TongDoanhThu)> LayThongKeAsync();
}

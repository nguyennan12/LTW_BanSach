using BanSach.Models;
using BanSach.ViewModels;

namespace BanSach.Services.Interfaces;

public interface IDonHangService
{
    Task<(bool ThanhCong, string? ThongBaoLoi, DonHang? DonHang)> TaoDonHangAsync(DatHangViewModel model, int? nguoiDungId = null);
    Task<DonHang?> LayDonHangTheoIdAsync(int id);
    Task<List<DonHang>> LayDanhSachDonHangAsync(string? trangThai = null, string? timKiem = null);
    Task<List<DonHang>> LayDanhSachDonHangCuaUserAsync(int nguoiDungId);
    Task<bool> CapNhatTrangThaiAsync(int id, string trangThaiMoi);
    Task<(int TongDon, int DonMoi, int DaGiao, long TongDoanhThu)> LayThongKeAsync();
}

using BanSach.Models;

namespace BanSach.Repositories.Interfaces;

public interface IDonHangRepository
{
    Task<DonHang> AddAsync(DonHang donHang);
    Task<DonHang?> GetByIdAsync(int id);
    Task<List<DonHang>> GetAllAsync(string? trangThai = null, string? timKiem = null);
    Task<List<DonHang>> GetByUserIdAsync(int userId);
    Task<bool> UpdateStatusAsync(int id, string trangThaiMoi);
    Task<(int TongDon, int DonMoi, int DaGiao, long TongDoanhThu)> GetStatsAsync();
}

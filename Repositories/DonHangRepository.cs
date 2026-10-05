using BanSach.Data;
using BanSach.Models;
using BanSach.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Repositories;

public class DonHangRepository : IDonHangRepository
{
    private readonly AppDbContext _db;

    public DonHangRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DonHang> AddAsync(DonHang donHang)
    {
        _db.DonHang.Add(donHang);
        await _db.SaveChangesAsync();
        return donHang;
    }

    public async Task<DonHang?> GetByIdAsync(int id)
    {
        return await _db.DonHang
            .Include(d => d.Sach)
            .Include(d => d.NguoiDung)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<DonHang>> GetAllAsync(string? trangThai = null, string? timKiem = null)
    {
        var query = _db.DonHang
            .Include(d => d.Sach)
            .Include(d => d.NguoiDung)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "Tất cả")
        {
            query = query.Where(d => d.TrangThai == trangThai);
        }

        if (!string.IsNullOrWhiteSpace(timKiem))
        {
            var key = timKiem.Trim().ToLower();
            query = query.Where(d => d.HoTen.ToLower().Contains(key) ||
                                     d.SoDienThoai.Contains(key) ||
                                     d.DiaChi.ToLower().Contains(key) ||
                                     d.Id.ToString() == key);
        }

        return await query.OrderByDescending(d => d.Id).ToListAsync();
    }

    public async Task<List<DonHang>> GetByUserIdAsync(int userId)
    {
        return await _db.DonHang
            .Include(d => d.Sach)
            .Where(d => d.NguoiDungId == userId)
            .OrderByDescending(d => d.Id)
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(int id, string trangThaiMoi)
    {
        var don = await _db.DonHang.FindAsync(id);
        if (don == null) return false;

        don.TrangThai = trangThaiMoi;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<(int TongDon, int DonMoi, int DaGiao, long TongDoanhThu)> GetStatsAsync()
    {
        var tatCa = await _db.DonHang.ToListAsync();
        int tongDon = tatCa.Count;
        int donMoi = tatCa.Count(d => d.TrangThai == "Chờ xử lý");
        int daGiao = tatCa.Count(d => d.TrangThai == "Đã giao");
        long tongDoanhThu = tatCa.Where(d => d.TrangThai != "Đã hủy").Sum(d => d.TongTien);

        return (tongDon, donMoi, daGiao, tongDoanhThu);
    }
}

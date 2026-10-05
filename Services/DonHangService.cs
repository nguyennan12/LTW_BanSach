using BanSach.Data;
using BanSach.Models;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Services;

public class DonHangService : IDonHangService
{
    private readonly AppDbContext _db;

    public DonHangService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(bool ThanhCong, string? ThongBaoLoi, DonHang? DonHang)> TaoDonHangAsync(DatHangViewModel model, int? nguoiDungId = null)
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

        if (model.SoLuong > sach.SoLuongTon)
        {
            return (false, $"Số lượng trong kho chỉ còn {sach.SoLuongTon} cuốn, không đủ để giao.", null);
        }

        var donHang = new DonHang
        {
            SachId = sach.Id,
            NguoiDungId = nguoiDungId ?? model.NguoiDungId,
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

        // Giảm số lượng tồn kho
        sach.SoLuongTon -= model.SoLuong;
        _db.DonHang.Add(donHang);
        await _db.SaveChangesAsync();

        return (true, null, donHang);
    }

    public async Task<DonHang?> LayDonHangTheoIdAsync(int id)
    {
        return await _db.DonHang
            .Include(d => d.Sach)
            .Include(d => d.NguoiDung)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<DonHang>> LayDanhSachDonHangAsync(string? trangThai = null, string? timKiem = null)
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

    public async Task<List<DonHang>> LayDanhSachDonHangCuaUserAsync(int nguoiDungId)
    {
        return await _db.DonHang
            .Include(d => d.Sach)
            .Where(d => d.NguoiDungId == nguoiDungId)
            .OrderByDescending(d => d.Id)
            .ToListAsync();
    }

    public async Task<bool> CapNhatTrangThaiAsync(int id, string trangThaiMoi)
    {
        var don = await _db.DonHang.FindAsync(id);
        if (don == null) return false;

        don.TrangThai = trangThaiMoi;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<(int TongDon, int DonMoi, int DaGiao, long TongDoanhThu)> LayThongKeAsync()
    {
        var tatCa = await _db.DonHang.ToListAsync();
        int tongDon = tatCa.Count;
        int donMoi = tatCa.Count(d => d.TrangThai == "Chờ xử lý");
        int daGiao = tatCa.Count(d => d.TrangThai == "Đã giao");
        long tongDoanhThu = tatCa.Where(d => d.TrangThai != "Đã hủy").Sum(d => d.TongTien);

        return (tongDon, donMoi, daGiao, tongDoanhThu);
    }
}

using BanSach.Models;
using BanSach.Repositories.Interfaces;
using BanSach.Services.Interfaces;
using BanSach.ViewModels;

namespace BanSach.Services;

public class DonHangService : IDonHangService
{
    private readonly IDonHangRepository _donHangRepo;
    private readonly ISachRepository _sachRepo;

    public DonHangService(IDonHangRepository donHangRepo, ISachRepository sachRepo)
    {
        _donHangRepo = donHangRepo;
        _sachRepo = sachRepo;
    }

    public async Task<(bool ThanhCong, string? ThongBaoLoi, DonHang? DonHang)> TaoDonHangAsync(DatHangViewModel model, int? nguoiDungId = null)
    {
        var sach = await _sachRepo.GetFirstAsync();
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

        // Giảm số lượng tồn kho của sách thông qua SachRepository
        sach.SoLuongTon -= model.SoLuong;
        await _sachRepo.UpdateAsync(sach);

        // Lưu đơn hàng qua DonHangRepository
        var createdDonHang = await _donHangRepo.AddAsync(donHang);

        return (true, null, createdDonHang);
    }

    public async Task<DonHang?> LayDonHangTheoIdAsync(int id)
    {
        return await _donHangRepo.GetByIdAsync(id);
    }

    public async Task<List<DonHang>> LayDanhSachDonHangAsync(string? trangThai = null, string? timKiem = null)
    {
        return await _donHangRepo.GetAllAsync(trangThai, timKiem);
    }

    public async Task<List<DonHang>> LayDanhSachDonHangCuaUserAsync(int nguoiDungId)
    {
        return await _donHangRepo.GetByUserIdAsync(nguoiDungId);
    }

    public async Task<bool> CapNhatTrangThaiAsync(int id, string trangThaiMoi)
    {
        return await _donHangRepo.UpdateStatusAsync(id, trangThaiMoi);
    }

    public async Task<(int TongDon, int DonMoi, int DaGiao, long TongDoanhThu)> LayThongKeAsync()
    {
        return await _donHangRepo.GetStatsAsync();
    }
}

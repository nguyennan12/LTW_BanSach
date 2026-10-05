using BanSach.Models;
using BanSach.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BanSach.Controllers;

[Authorize(Roles = "Admin")]
public class QuanTriController : Controller
{
    private readonly IDonHangService _donHangService;
    private readonly ISachService _sachService;
    private readonly IFeedbackService _feedbackService;

    public QuanTriController(
        IDonHangService donHangService,
        ISachService sachService,
        IFeedbackService feedbackService)
    {
        _donHangService = donHangService;
        _sachService = sachService;
        _feedbackService = feedbackService;
    }

    // Dashboard Quản trị hóa đơn & đơn hàng: GET /QuanTri
    public async Task<IActionResult> Index(string? trangThai, string? timKiem)
    {
        var dsDonHang = await _donHangService.LayDanhSachDonHangAsync(trangThai, timKiem);
        var (tongDon, donMoi, daGiao, tongDoanhThu) = await _donHangService.LayThongKeAsync();

        ViewBag.TrangThai = trangThai ?? "Tất cả";
        ViewBag.TimKiem = timKiem ?? "";
        ViewBag.TongDon = tongDon;
        ViewBag.DonMoi = donMoi;
        ViewBag.DaGiao = daGiao;
        ViewBag.TongDoanhThu = tongDoanhThu;

        return View(dsDonHang);
    }

    // Xem Chi tiết hóa đơn: GET /QuanTri/ChiTietHoaDon/5
    public async Task<IActionResult> ChiTietHoaDon(int id)
    {
        var don = await _donHangService.LayDonHangTheoIdAsync(id);
        if (don == null) return NotFound("Không tìm thấy hóa đơn này.");
        return View(don);
    }

    // Cập nhật trạng thái đơn hàng: POST /QuanTri/CapNhatTrangThai
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(int id, string trangThaiMoi, string? trangThaiHienTai, string? timKiem)
    {
        await _donHangService.CapNhatTrangThaiAsync(id, trangThaiMoi);
        TempData["SuccessMsg"] = $"Đã cập nhật hóa đơn #{id} sang trạng thái '{trangThaiMoi}'.";
        return RedirectToAction(nameof(Index), new { trangThai = trangThaiHienTai, timKiem });
    }

    // Quản lý Đánh giá Feedback: GET /QuanTri/QuanLyFeedback
    public async Task<IActionResult> QuanLyFeedback()
    {
        var ds = await _feedbackService.LayTatCaFeedbackAsync();
        return View(ds);
    }

    // Duyệt / Ẩn Feedback
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DuyetFeedback(int id, bool duyet)
    {
        await _feedbackService.DuyetFeedbackAsync(id, duyet);
        return RedirectToAction(nameof(QuanLyFeedback));
    }

    // Xóa Feedback
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaFeedback(int id)
    {
        await _feedbackService.XoaFeedbackAsync(id);
        return RedirectToAction(nameof(QuanLyFeedback));
    }
}

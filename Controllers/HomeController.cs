using System.Security.Claims;
using BanSach.Models;
using BanSach.Services.Interfaces;
using BanSach.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BanSach.Controllers;

public class HomeController : Controller
{
    private readonly ISachService _sachService;
    private readonly IDonHangService _donHangService;
    private readonly IFeedbackService _feedbackService;
    private readonly IAuthService _authService;

    public HomeController(
        ISachService sachService,
        IDonHangService donHangService,
        IFeedbackService feedbackService,
        IAuthService authService)
    {
        _sachService = sachService;
        _donHangService = donHangService;
        _feedbackService = feedbackService;
        _authService = authService;
    }

    // Trang chủ Landing Page: GET /
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sach = await _sachService.LaySachChinhAsync();
        if (sach == null) return NotFound("Chưa khởi tạo dữ liệu sách.");

        var feedbacks = await _feedbackService.LayFeedbackDaDuyetAsync(sach.Id);

        var datHangForm = new DatHangViewModel { Sach = sach, SoLuong = 1 };

        // Nếu người dùng đã đăng nhập -> Tự động điền thông tin người dùng
        if (User.Identity?.IsAuthenticated == true)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                var user = await _authService.LayNguoiDungTheoIdAsync(userId);
                if (user != null)
                {
                    datHangForm.NguoiDungId = user.Id;
                    datHangForm.HoTen = user.HoTen;
                    datHangForm.SoDienThoai = user.SoDienThoai;
                    datHangForm.DiaChi = user.DiaChi;
                }
            }
        }

        var vm = new LandingPageViewModel
        {
            Sach = sach,
            Feedbacks = feedbacks,
            DatHangForm = datHangForm
        };

        return View(vm);
    }

    // Xử lý gửi Form Đặt Hàng: POST /Home/DatHang
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatHang(DatHangViewModel datHangForm)
    {
        var sach = await _sachService.LaySachChinhAsync();
        if (sach == null) return NotFound();

        datHangForm.Sach = sach;

        // BẮT BUỘC ĐĂNG NHẬP KHI MUA SÁCH THEO YÊU CẦU
        if (User.Identity?.IsAuthenticated != true)
        {
            TempData["ErrorMsg"] = "Bạn cần đăng nhập tài khoản trước khi đặt mua sách!";
            return RedirectToAction("DangNhap", "Account", new { returnUrl = "/#dat-hang" });
        }

        // Lấy User ID của tài khoản đang đăng nhập
        int? userId = null;
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdStr, out var parsedId))
        {
            userId = parsedId;
            datHangForm.NguoiDungId = parsedId;
        }

        if (!ModelState.IsValid)
        {
            var feedbacks = await _feedbackService.LayFeedbackDaDuyetAsync(sach.Id);
            var vm = new LandingPageViewModel
            {
                Sach = sach,
                Feedbacks = feedbacks,
                DatHangForm = datHangForm
            };
            return View("Index", vm);
        }

        var (thanhCong, loi, donHang) = await _donHangService.TaoDonHangAsync(datHangForm, userId);
        if (!thanhCong || donHang == null)
        {
            ModelState.AddModelError(string.Empty, loi ?? "Đã xảy ra lỗi khi tạo đơn hàng. Vui lòng thử lại!");
            var feedbacks = await _feedbackService.LayFeedbackDaDuyetAsync(sach.Id);
            var vm = new LandingPageViewModel
            {
                Sach = sach,
                Feedbacks = feedbacks,
                DatHangForm = datHangForm
            };
            return View("Index", vm);
        }

        return RedirectToAction(nameof(CamOn), new { id = donHang.Id });
    }

    // Trang Cảm Ơn / Hóa Đơn Chi Tiết: GET /Home/CamOn/5
    [HttpGet]
    public async Task<IActionResult> CamOn(int id)
    {
        var don = await _donHangService.LayDonHangTheoIdAsync(id);
        if (don == null) return NotFound();
        return View(don);
    }

    // Gửi đánh giá Feedback: POST /Home/GuiFeedback
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GuiFeedback(FeedbackViewModel feedbackForm)
    {
        if (ModelState.IsValid)
        {
            await _feedbackService.GuiFeedbackAsync(feedbackForm);
            TempData["SuccessMsg"] = "Cảm ơn bạn đã gửi đánh giá! Nhận xét của bạn đã được ghi nhận.";
        }
        else
        {
            TempData["ErrorMsg"] = "Vui lòng nhập đầy đủ thông tin đánh giá.";
        }

        return Redirect("/#feedback");
    }
}

using System.Security.Claims;
using BanSach.Models;
using BanSach.Services.Interfaces;
using BanSach.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BanSach.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;
    private readonly IDonHangService _donHangService;

    public AccountController(
        IAuthService authService,
        IJwtService jwtService,
        IDonHangService donHangService)
    {
        _authService = authService;
        _jwtService = jwtService;
        _donHangService = donHangService;
    }

    // GET: /Account/DangNhap
    [HttpGet]
    [AllowAnonymous]
    public IActionResult DangNhap(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("Admin")) return RedirectToAction("Index", "QuanTri");
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // POST: /Account/DangNhap
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangNhap(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.DangNhapAsync(model);
        if (!result.Success || result.User == null)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Đăng nhập thất bại.");
            return View(model);
        }

        // Lưu JWT token vào Cookie
        Response.Cookies.Append("access_token", result.Token ?? "", new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = model.GhiNho ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(12)
        });

        // Thiết lập Cookie Authentication cho MVC Controller Claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
            new(ClaimTypes.Name, result.User.TenDangNhap),
            new(ClaimTypes.GivenName, result.User.HoTen),
            new(ClaimTypes.Role, result.User.VaiTro),
            new(ClaimTypes.MobilePhone, result.User.SoDienThoai ?? ""),
            new(ClaimTypes.StreetAddress, result.User.DiaChi ?? ""),
            new("jwt_token", result.Token ?? "")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.GhiNho,
            ExpiresUtc = model.GhiNho ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(12)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            authProperties);

        TempData["SuccessMsg"] = $"Chào mừng {result.User.HoTen} đã đăng nhập thành công!";

        // Chuyển hướng theo vai trò hoặc ReturnUrl
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        if (result.User.VaiTro == "Admin")
        {
            return RedirectToAction("Index", "QuanTri");
        }

        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/DangKy
    [HttpGet]
    [AllowAnonymous]
    public IActionResult DangKy()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(new RegisterViewModel());
    }

    // POST: /Account/DangKy
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangKy(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.DangKyAsync(model);
        if (!result.Success || result.User == null)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Đăng ký thất bại.");
            return View(model);
        }

        // Tự động đăng nhập sau khi đăng ký thành công
        Response.Cookies.Append("access_token", result.Token ?? "", new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
            new(ClaimTypes.Name, result.User.TenDangNhap),
            new(ClaimTypes.GivenName, result.User.HoTen),
            new(ClaimTypes.Role, result.User.VaiTro),
            new(ClaimTypes.MobilePhone, result.User.SoDienThoai ?? ""),
            new(ClaimTypes.StreetAddress, result.User.DiaChi ?? ""),
            new("jwt_token", result.Token ?? "")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        TempData["SuccessMsg"] = "Đăng ký tài khoản thành công! Bạn có thể đặt mua sách ngay.";
        return RedirectToAction("Index", "Home");
    }

    // POST: /Account/DangXuat
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangXuat()
    {
        Response.Cookies.Delete("access_token");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["SuccessMsg"] = "Đã đăng xuất thành công.";
        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/LichSuDonHang (Xem lịch sử mua sách của User)
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> LichSuDonHang()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return RedirectToAction(nameof(DangNhap));
        }

        var danhSachDon = await _donHangService.LayDanhSachDonHangCuaUserAsync(userId);
        return View(danhSachDon);
    }

    // API Endpoint: POST /Account/Api/Login (Trả về JWT Token định dạng JSON)
    [HttpPost("Account/Api/Login")]
    [AllowAnonymous]
    public async Task<IActionResult> ApiLogin([FromBody] LoginViewModel model)
    {
        var result = await _authService.DangNhapAsync(model);
        if (!result.Success)
        {
            return Unauthorized(new { success = false, message = result.ErrorMessage });
        }

        return Ok(new
        {
            success = true,
            token = result.Token,
            user = new
            {
                id = result.User?.Id,
                username = result.User?.TenDangNhap,
                fullName = result.User?.HoTen,
                role = result.User?.VaiTro
            }
        });
    }
}

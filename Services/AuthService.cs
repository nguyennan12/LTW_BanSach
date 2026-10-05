using BanSach.Data;
using BanSach.Models;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IJwtService _jwtService;

    public AuthService(AppDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    public async Task<AuthResult> DangNhapAsync(LoginViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.TenDangNhap) || string.IsNullOrWhiteSpace(model.MatKhau))
        {
            return new AuthResult { Success = false, ErrorMessage = "Vui lòng điền đầy đủ tên đăng nhập và mật khẩu." };
        }

        var username = model.TenDangNhap.Trim().ToLower();
        var user = await _db.NguoiDung.FirstOrDefaultAsync(u => u.TenDangNhap.ToLower() == username);

        if (user == null || user.MatKhau != model.MatKhau)
        {
            return new AuthResult { Success = false, ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác." };
        }

        var token = _jwtService.TaoToken(user);
        return new AuthResult
        {
            Success = true,
            Token = token,
            User = user
        };
    }

    public async Task<AuthResult> DangKyAsync(RegisterViewModel model)
    {
        var username = model.TenDangNhap.Trim().ToLower();
        var exists = await _db.NguoiDung.AnyAsync(u => u.TenDangNhap.ToLower() == username);
        if (exists)
        {
            return new AuthResult { Success = false, ErrorMessage = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác." };
        }

        var newUser = new NguoiDung
        {
            TenDangNhap = model.TenDangNhap.Trim(),
            MatKhau = model.MatKhau,
            HoTen = model.HoTen.Trim(),
            SoDienThoai = model.SoDienThoai.Trim(),
            DiaChi = model.DiaChi.Trim(),
            VaiTro = "User", // Mặc định tài khoản đăng ký là User
            NgayTao = DateTime.Now
        };

        _db.NguoiDung.Add(newUser);
        await _db.SaveChangesAsync();

        var token = _jwtService.TaoToken(newUser);
        return new AuthResult
        {
            Success = true,
            Token = token,
            User = newUser
        };
    }

    public async Task<NguoiDung?> LayNguoiDungTheoIdAsync(int id)
    {
        return await _db.NguoiDung.FindAsync(id);
    }

    public async Task<NguoiDung?> LayNguoiDungTheoUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;
        var normalized = username.Trim().ToLower();
        return await _db.NguoiDung.FirstOrDefaultAsync(u => u.TenDangNhap.ToLower() == normalized);
    }
}

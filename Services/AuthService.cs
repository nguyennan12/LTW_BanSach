using BanSach.Models;
using BanSach.Repositories.Interfaces;
using BanSach.Services.Interfaces;
using BanSach.ViewModels;

namespace BanSach.Services;

public class AuthService : IAuthService
{
    private readonly INguoiDungRepository _userRepo;
    private readonly IJwtService _jwtService;

    public AuthService(INguoiDungRepository userRepo, IJwtService jwtService)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
    }

    public async Task<AuthResult> DangNhapAsync(LoginViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.TenDangNhap) || string.IsNullOrWhiteSpace(model.MatKhau))
        {
            return new AuthResult { Success = false, ErrorMessage = "Vui lòng điền đầy đủ tên đăng nhập và mật khẩu." };
        }

        var username = model.TenDangNhap.Trim().ToLower();
        var user = await _userRepo.GetByUsernameAsync(username);

        if (user == null)
        {
            return new AuthResult { Success = false, ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác." };
        }

        // Kiểm tra mật khẩu đã băm bằng thuật toán BCrypt
        bool hopLe = false;
        try
        {
            hopLe = BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhau);
        }
        catch
        {
            hopLe = (user.MatKhau == model.MatKhau);
            if (hopLe)
            {
                user.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
                await _userRepo.UpdateAsync(user);
            }
        }

        if (!hopLe)
        {
            return new AuthResult { Success = false, ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác." };
        }

        // Tạo JWT Token sau khi xác thực mật khẩu thành công
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
        var exists = await _userRepo.ExistsByUsernameAsync(username);
        if (exists)
        {
            return new AuthResult { Success = false, ErrorMessage = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác." };
        }

        // BĂM MẬT KHẨU BẰNG THUẬT TOÁN BCRYPT KÈM SALT NGẪU NHIÊN
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.MatKhau.Trim());

        var newUser = new NguoiDung
        {
            TenDangNhap = model.TenDangNhap.Trim(),
            MatKhau = hashedPassword,
            HoTen = model.HoTen.Trim(),
            SoDienThoai = model.SoDienThoai.Trim(),
            DiaChi = model.DiaChi.Trim(),
            VaiTro = "User",
            NgayTao = DateTime.Now
        };

        var createdUser = await _userRepo.AddAsync(newUser);

        var token = _jwtService.TaoToken(createdUser);
        return new AuthResult
        {
            Success = true,
            Token = token,
            User = createdUser
        };
    }

    public async Task<NguoiDung?> LayNguoiDungTheoIdAsync(int id)
    {
        return await _userRepo.GetByIdAsync(id);
    }

    public async Task<NguoiDung?> LayNguoiDungTheoUsernameAsync(string username)
    {
        return await _userRepo.GetByUsernameAsync(username);
    }
}

using BanSach.Models;
using BanSach.ViewModels;

namespace BanSach.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResult> DangNhapAsync(LoginViewModel model);
    Task<AuthResult> DangKyAsync(RegisterViewModel model);
    Task<NguoiDung?> LayNguoiDungTheoIdAsync(int id);
    Task<NguoiDung?> LayNguoiDungTheoUsernameAsync(string username);
}

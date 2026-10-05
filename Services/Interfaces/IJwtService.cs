using System.Security.Claims;
using BanSach.Models;

namespace BanSach.Services.Interfaces;

public interface IJwtService
{
    string TaoToken(NguoiDung user);
    ClaimsPrincipal? GiaiMaToken(string token);
}

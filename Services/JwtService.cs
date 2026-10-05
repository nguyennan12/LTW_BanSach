using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BanSach.Models;
using Microsoft.IdentityModel.Tokens;

namespace BanSach.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string TaoToken(NguoiDung user)
    {
        var key = _config["Jwt:Key"] ?? "BanSachSecretKeyForJwtAuthentication2026SuperSecureKey!";
        var issuer = _config["Jwt:Issuer"] ?? "BanSachMvc";
        var audience = _config["Jwt:Audience"] ?? "BanSachMvcClient";
        var expireDays = int.TryParse(_config["Jwt:ExpireDays"], out var days) ? days : 7;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.TenDangNhap),
            new(ClaimTypes.GivenName, user.HoTen),
            new(ClaimTypes.Role, user.VaiTro),
            new(ClaimTypes.MobilePhone, user.SoDienThoai ?? ""),
            new(ClaimTypes.StreetAddress, user.DiaChi ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expireDays),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? GiaiMaToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        var key = _config["Jwt:Key"] ?? "BanSachSecretKeyForJwtAuthentication2026SuperSecureKey!";
        var issuer = _config["Jwt:Issuer"] ?? "BanSachMvc";
        var audience = _config["Jwt:Audience"] ?? "BanSachMvcClient";

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}

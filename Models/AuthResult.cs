namespace BanSach.Models;

public class AuthResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Token { get; set; }
    public NguoiDung? User { get; set; }
}

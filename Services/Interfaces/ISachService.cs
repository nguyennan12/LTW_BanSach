using BanSach.Models;

namespace BanSach.Services.Interfaces;

public interface ISachService
{
    Task<Sach?> LaySachChinhAsync();
    Task<Sach?> LaySachTheoIdAsync(int id);
    Task CapNhatSachAsync(Sach sach);
}

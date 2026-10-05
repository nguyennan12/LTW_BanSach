using BanSach.Models;

namespace BanSach.Repositories.Interfaces;

public interface ISachRepository
{
    Task<Sach?> GetFirstAsync();
    Task<Sach?> GetByIdAsync(int id);
    Task UpdateAsync(Sach sach);
}

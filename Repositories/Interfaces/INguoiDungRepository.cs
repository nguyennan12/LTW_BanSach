using BanSach.Models;

namespace BanSach.Repositories.Interfaces;

public interface INguoiDungRepository
{
    Task<NguoiDung?> GetByIdAsync(int id);
    Task<NguoiDung?> GetByUsernameAsync(string username);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<NguoiDung> AddAsync(NguoiDung user);
    Task UpdateAsync(NguoiDung user);
}

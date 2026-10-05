using BanSach.Models;
using BanSach.Repositories.Interfaces;
using BanSach.Services.Interfaces;

namespace BanSach.Services;

public class SachService : ISachService
{
    private readonly ISachRepository _sachRepo;

    public SachService(ISachRepository sachRepo)
    {
        _sachRepo = sachRepo;
    }

    public async Task<Sach?> LaySachChinhAsync()
    {
        return await _sachRepo.GetFirstAsync();
    }

    public async Task<Sach?> LaySachTheoIdAsync(int id)
    {
        return await _sachRepo.GetByIdAsync(id);
    }

    public async Task CapNhatSachAsync(Sach sach)
    {
        await _sachRepo.UpdateAsync(sach);
    }
}

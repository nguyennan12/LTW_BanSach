using BanSach.Data;
using BanSach.Models;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Services;

public class SachService : ISachService
{
    private readonly AppDbContext _db;

    public SachService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Sach?> LaySachChinhAsync()
    {
        return await _db.Sach.FirstOrDefaultAsync();
    }

    public async Task<Sach?> LaySachTheoIdAsync(int id)
    {
        return await _db.Sach.FindAsync(id);
    }

    public async Task CapNhatSachAsync(Sach sach)
    {
        _db.Sach.Update(sach);
        await _db.SaveChangesAsync();
    }
}

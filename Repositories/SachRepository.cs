using BanSach.Data;
using BanSach.Models;
using BanSach.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Repositories;

public class SachRepository : ISachRepository
{
    private readonly AppDbContext _db;

    public SachRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Sach?> GetFirstAsync()
    {
        return await _db.Sach.FirstOrDefaultAsync();
    }

    public async Task<Sach?> GetByIdAsync(int id)
    {
        return await _db.Sach.FindAsync(id);
    }

    public async Task UpdateAsync(Sach sach)
    {
        _db.Sach.Update(sach);
        await _db.SaveChangesAsync();
    }
}

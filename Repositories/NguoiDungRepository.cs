using BanSach.Data;
using BanSach.Models;
using BanSach.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Repositories;

public class NguoiDungRepository : INguoiDungRepository
{
    private readonly AppDbContext _db;

    public NguoiDungRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<NguoiDung?> GetByIdAsync(int id)
    {
        return await _db.NguoiDung.FindAsync(id);
    }

    public async Task<NguoiDung?> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;
        var normalized = username.Trim().ToLower();
        return await _db.NguoiDung.FirstOrDefaultAsync(u => u.TenDangNhap.ToLower() == normalized);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return false;
        var normalized = username.Trim().ToLower();
        return await _db.NguoiDung.AnyAsync(u => u.TenDangNhap.ToLower() == normalized);
    }

    public async Task<NguoiDung> AddAsync(NguoiDung user)
    {
        _db.NguoiDung.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(NguoiDung user)
    {
        _db.NguoiDung.Update(user);
        await _db.SaveChangesAsync();
    }
}

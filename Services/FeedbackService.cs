using BanSach.Data;
using BanSach.Models;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Services;

public class FeedbackService : IFeedbackService
{
    private readonly AppDbContext _db;

    public FeedbackService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Feedback>> LayFeedbackDaDuyetAsync(int sachId = 1)
    {
        return await _db.Feedback
            .Where(f => f.SachId == sachId && f.Duyet)
            .OrderByDescending(f => f.Id)
            .ToListAsync();
    }

    public async Task<List<Feedback>> LayTatCaFeedbackAsync()
    {
        return await _db.Feedback
            .Include(f => f.Sach)
            .OrderByDescending(f => f.Id)
            .ToListAsync();
    }

    public async Task<bool> GuiFeedbackAsync(FeedbackViewModel model, int sachId = 1)
    {
        var feedback = new Feedback
        {
            SachId = sachId,
            HoTen = model.HoTen.Trim(),
            SoSao = Math.Clamp(model.SoSao, 1, 5),
            NoiDung = model.NoiDung.Trim(),
            NgayDanhGia = DateTime.Now,
            Duyet = true // Mặc định tự động duyệt hoặc duyệt tay
        };

        _db.Feedback.Add(feedback);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DuyetFeedbackAsync(int id, bool duyet)
    {
        var fb = await _db.Feedback.FindAsync(id);
        if (fb == null) return false;

        fb.Duyet = duyet;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> XoaFeedbackAsync(int id)
    {
        var fb = await _db.Feedback.FindAsync(id);
        if (fb == null) return false;

        _db.Feedback.Remove(fb);
        await _db.SaveChangesAsync();
        return true;
    }
}

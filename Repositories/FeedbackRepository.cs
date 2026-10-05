using BanSach.Data;
using BanSach.Models;
using BanSach.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly AppDbContext _db;

    public FeedbackRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Feedback>> GetApprovedByBookIdAsync(int sachId = 1)
    {
        return await _db.Feedback
            .Where(f => f.SachId == sachId && f.Duyet)
            .OrderByDescending(f => f.Id)
            .ToListAsync();
    }

    public async Task<List<Feedback>> GetAllAsync()
    {
        return await _db.Feedback
            .Include(f => f.Sach)
            .OrderByDescending(f => f.Id)
            .ToListAsync();
    }

    public async Task<Feedback> AddAsync(Feedback feedback)
    {
        _db.Feedback.Add(feedback);
        await _db.SaveChangesAsync();
        return feedback;
    }

    public async Task<bool> UpdateApprovalAsync(int id, bool duyet)
    {
        var fb = await _db.Feedback.FindAsync(id);
        if (fb == null) return false;

        fb.Duyet = duyet;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var fb = await _db.Feedback.FindAsync(id);
        if (fb == null) return false;

        _db.Feedback.Remove(fb);
        await _db.SaveChangesAsync();
        return true;
    }
}

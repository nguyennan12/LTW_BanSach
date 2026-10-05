using BanSach.Models;
using BanSach.Repositories.Interfaces;
using BanSach.Services.Interfaces;
using BanSach.ViewModels;

namespace BanSach.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepo;

    public FeedbackService(IFeedbackRepository feedbackRepo)
    {
        _feedbackRepo = feedbackRepo;
    }

    public async Task<List<Feedback>> LayFeedbackDaDuyetAsync(int sachId = 1)
    {
        return await _feedbackRepo.GetApprovedByBookIdAsync(sachId);
    }

    public async Task<List<Feedback>> LayTatCaFeedbackAsync()
    {
        return await _feedbackRepo.GetAllAsync();
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
            Duyet = true
        };

        await _feedbackRepo.AddAsync(feedback);
        return true;
    }

    public async Task<bool> DuyetFeedbackAsync(int id, bool duyet)
    {
        return await _feedbackRepo.UpdateApprovalAsync(id, duyet);
    }

    public async Task<bool> XoaFeedbackAsync(int id)
    {
        return await _feedbackRepo.DeleteAsync(id);
    }
}

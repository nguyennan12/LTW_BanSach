using BanSach.Models;

namespace BanSach.Services;

public interface IFeedbackService
{
    Task<List<Feedback>> LayFeedbackDaDuyetAsync(int sachId = 1);
    Task<List<Feedback>> LayTatCaFeedbackAsync();
    Task<bool> GuiFeedbackAsync(FeedbackViewModel model, int sachId = 1);
    Task<bool> DuyetFeedbackAsync(int id, bool duyet);
    Task<bool> XoaFeedbackAsync(int id);
}

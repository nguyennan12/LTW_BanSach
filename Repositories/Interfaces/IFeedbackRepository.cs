using BanSach.Models;

namespace BanSach.Repositories.Interfaces;

public interface IFeedbackRepository
{
    Task<List<Feedback>> GetApprovedByBookIdAsync(int sachId = 1);
    Task<List<Feedback>> GetAllAsync();
    Task<Feedback> AddAsync(Feedback feedback);
    Task<bool> UpdateApprovalAsync(int id, bool duyet);
    Task<bool> DeleteAsync(int id);
}

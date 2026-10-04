namespace BanSach.Models;

public class LandingPageViewModel
{
    public Sach Sach { get; set; } = new();
    public List<Feedback> Feedbacks { get; set; } = new();
    public DatHangViewModel DatHangForm { get; set; } = new();
    public FeedbackViewModel FeedbackForm { get; set; } = new();
}

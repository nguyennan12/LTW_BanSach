using System.ComponentModel.DataAnnotations;

namespace BanSach.Models;

public class FeedbackViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên của bạn")]
    [StringLength(50)]
    public string HoTen { get; set; } = "";

    [Range(1, 5, ErrorMessage = "Đánh giá từ 1 đến 5 sao")]
    public int SoSao { get; set; } = 5;

    [Required(ErrorMessage = "Vui lòng để lại cảm nhận của bạn")]
    [StringLength(500, ErrorMessage = "Đánh giá tối đa 500 ký tự")]
    public string NoiDung { get; set; } = "";
}

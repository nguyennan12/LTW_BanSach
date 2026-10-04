namespace BanSach.Models;

public class Feedback
{
    public int Id { get; set; }
    public int SachId { get; set; }
    public Sach? Sach { get; set; }

    public string HoTen { get; set; } = "";
    public int SoSao { get; set; } = 5;
    public string NoiDung { get; set; } = "";
    public DateTime NgayDanhGia { get; set; } = DateTime.Now;
    public bool Duyet { get; set; } = true;
}

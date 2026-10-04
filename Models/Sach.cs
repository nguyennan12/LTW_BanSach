namespace BanSach.Models;

public class Sach
{
    public int Id { get; set; }
    public string Ten { get; set; } = "";
    public string TheLoai { get; set; } = "Tiểu thuyết / Triết lý sống";
    public string TacGia { get; set; } = "Paulo Coelho";
    public string ThongTinTacGia { get; set; } = "";
    public string Title { get; set; } = "Hành Trình Theo Đuổi Ước Mơ & Định Mệnh";
    public string Subtitle { get; set; } = "Khi bạn khao khát một điều gì đó, cả vũ trụ sẽ hợp lực giúp bạn đạt được điều đó.";
    public string MoTa { get; set; } = "";
    public string ReviewNoiDung { get; set; } = "";
    public long GiaGoc { get; set; } = 120000;
    public long Gia { get; set; } = 89000;          // Giá bán khuyến mãi
    public int SoLuongTon { get; set; } = 100;
    public string AnhBia { get; set; } = "/img/nhagiakim.jpg";
    public string FilePreview { get; set; } = "#doc-thu";
}

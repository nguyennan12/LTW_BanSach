using BanSach.Models;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Sach> Sach => Set<Sach>();
    public DbSet<DonHang> DonHang => Set<DonHang>();
    public DbSet<Feedback> Feedback => Set<Feedback>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Seed thông tin sách "Nhà Giả Kim"
        b.Entity<Sach>().HasData(new Sach
        {
            Id = 1,
            Ten = "Nhà Giả Kim (The Alchemist)",
            TheLoai = "Tiểu thuyết triết lý, Truyền cảm hứng",
            TacGia = "Paulo Coelho",
            ThongTinTacGia = "Paulo Coelho (sinh ngày 24/8/1947 tại Rio de Janeiro, Brasil) là một trong những nhà văn có tác phẩm được đọc nhiều nhất thế giới. Sách của ông được dịch ra hơn 80 thứ tiếng và phát hành tại hơn 170 quốc gia với hàng trăm triệu bản.",
            Title = "Tuyệt Tác Thay Đổi Cuộc Đời Của Hàng Triệu Người",
            Subtitle = "Khi bạn khao khát một điều gì đó, cả vũ trụ sẽ hợp lực giúp bạn đạt được điều đó.",
            MoTa = "Nhà Giả Kim kể về chuyến phiêu lưu theo đuổi vận mệnh của chàng chăn cừu Santiago. Cuốn sách như một câu chuyện cổ tích giản dị nhưng ẩn chứa những minh triết sâu sắc về ước mơ, lòng dũng cảm và ngôn ngữ của vũ trụ.",
            ReviewNoiDung = "Cuốn sách mở ra bí mật về 'Vận Mệnh Cá Nhân' (Personal Legend) - điều mà mỗi người sinh ra đều có bổn phận phải thực hiện. Dù là băng qua sa mạc, vượt qua nỗi sợ hãi thất bại hay lắng nghe tiếng gọi con tim, Santiago đã chứng minh rằng: kho báu vĩ đại nhất đôi khi nằm ngay tại điểm xuất phát, nhưng bạn phải đi hết một vòng hành trình mới nhận ra.",
            GiaGoc = 120000,
            Gia = 89000,
            SoLuongTon = 150,
            AnhBia = "/img/nhagiakim.jpg",
            FilePreview = "#doc-thu"
        });

        // Seed feedback mẫu
        b.Entity<Feedback>().HasData(
            new Feedback
            {
                Id = 1,
                SachId = 1,
                HoTen = "Nguyễn Minh Tuấn",
                SoSao = 5,
                NoiDung = "Cuốn sách đã tiếp thêm cho mình rất nhiều niềm tin để theo đuổi con đường khởi nghiệp. Đọc đi đọc lại vẫn thấy thấm!",
                NgayDanhGia = new DateTime(2026, 9, 15),
                Duyet = true
            },
            new Feedback
            {
                Id = 2,
                SachId = 1,
                HoTen = "Trần Thị Mai Phương",
                SoSao = 5,
                NoiDung = "Giao hàng cực kỳ nhanh, sách được bọc màng co cẩn thận, bìa đẹp và giấy ngà chống lóa rất êm mắt. Rất hài lòng!",
                NgayDanhGia = new DateTime(2026, 9, 20),
                Duyet = true
            },
            new Feedback
            {
                Id = 3,
                SachId = 1,
                HoTen = "Lê Hoàng Nam",
                SoSao = 5,
                NoiDung = "'Khi bạn khao khát một điều gì đó, cả vũ trụ sẽ hợp lực...' - Câu nói làm thay đổi hoàn toàn cách nhìn cuộc sống của mình.",
                NgayDanhGia = new DateTime(2026, 9, 28),
                Duyet = true
            }
        );
    }
}

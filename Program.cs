using BanSach.Data;
using BanSach.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Database SQLite
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Đăng ký các tầng Services (Dependency Injection)
builder.Services.AddScoped<ISachService, SachService>();
builder.Services.AddScoped<IDonHangService, DonHangService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Tự động khởi tạo Database và dữ liệu mẫu sách lần đầu
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

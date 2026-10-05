using System.Text;
using BanSach.Data;
using BanSach.Repositories;
using BanSach.Repositories.Interfaces;
using BanSach.Services;
using BanSach.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Database SQLite
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// 2. Đăng ký Dependency Injection cho tầng Repositories (Data Access Layer)
builder.Services.AddScoped<ISachRepository, SachRepository>();
builder.Services.AddScoped<IDonHangRepository, DonHangRepository>();
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();

// 3. Đăng ký Dependency Injection cho tầng Services (Business Logic Layer)
builder.Services.AddScoped<ISachService, SachService>();
builder.Services.AddScoped<IDonHangService, DonHangService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 4. Cấu hình Xác thực Đăng Nhập (Cookie + JWT Bearer)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "BanSachSecretKeyForJwtAuthentication2026SuperSecureKey!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BanSachMvc";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BanSachMvcClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Account/DangNhap";
    options.LogoutPath = "/Account/DangXuat";
    options.AccessDeniedPath = "/Account/DangNhap";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 5. Khởi tạo Database và BĂM TOÀN BỘ MẬT KHẨU TRONG BẢNG NGUOIDUNG BẰNG BCRYPT
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

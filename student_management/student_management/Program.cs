using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using student_management.Models;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Kết nối tới CSDL
builder.Services.AddDbContext<QuanlyhocDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 🔹 Thêm session (để lưu thông tin đăng nhập)
builder.Services.AddSession();

// 🔹 Cấu hình xác thực Cookie (đăng nhập)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/TaiKhoan/DangNhap"; // đường dẫn tới trang đăng nhập
        options.LogoutPath = "/TaiKhoan/DangXuat";
        options.AccessDeniedPath = "/TaiKhoan/KhongCoQuyen"; // khi người dùng bị chặn quyền truy cập
    });

// 🔹 Add controllers + views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔹 Cấu hình pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ✅✅ Thêm đoạn cấu hình ngôn ngữ mặc định là Tiếng Việt
var supportedCultures = new[] { new CultureInfo("vi-VN") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("vi-VN"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
app.UseRequestLocalization(localizationOptions);
// ✅✅ Hết đoạn cần thêm

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔹 Bật session & xác thực
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// 🔹 Định tuyến cho Area
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// 🔹 Định tuyến mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();

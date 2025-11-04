using Microsoft.AspNetCore.Mvc;
using student_management.Models;
using student_management.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace student_management.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public AccountController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 🔍 Kiểm tra tài khoản
            var user = await _context.TaiKhoans
                .Include(t => t.MaVaiTroNavigation)
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap && t.TrangThai == true);

            if (user == null || user.MatKhau != model.MatKhau)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View(model);
            }

            // ✅ Tạo claims (xác thực cookie)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.TenDangNhap),
                new Claim(ClaimTypes.Role, user.MaVaiTroNavigation.TenVaiTro),
                new Claim("UserId", user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // ✅ Điều hướng theo vai trò
            if (user.MaVaiTro == 1)
            {
                // ADMIN
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else if (user.MaVaiTro == 2)
            {
                // GIẢNG VIÊN
                var giangVien = await _context.GiaoViens
                    .FirstOrDefaultAsync(g => g.MaGv == user.MaGv);

                if (giangVien != null)
                {
                    HttpContext.Session.SetString("MaGV", giangVien.MaGv);
                    HttpContext.Session.SetString("HoTenGV", giangVien.HoTen);
                }

                return RedirectToAction("Index", "Home", new { area = "GiangVien" });
            }
            else
            {
                // SINH VIÊN
                var sinhVien = await _context.SinhViens
                    .FirstOrDefaultAsync(s => s.MaSv == user.MaSv);

                if (sinhVien != null)
                {
                    HttpContext.Session.SetString("MaSV", sinhVien.MaSv);
                    HttpContext.Session.SetString("HoTenSV", sinhVien.HoTen);
                }

                // 👉 Sinh viên KHÔNG nằm trong area, nên không thêm "area"
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}

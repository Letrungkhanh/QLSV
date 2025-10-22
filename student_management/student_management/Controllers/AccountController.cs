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

            var user = await _context.TaiKhoans
                .Include(t => t.MaVaiTroNavigation)
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap && t.TrangThai == true);

            if (user == null || user.MatKhau != model.MatKhau)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.TenDangNhap),
                new Claim(ClaimTypes.Role, user.MaVaiTroNavigation.TenVaiTro),
                new Claim("UserId", user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Điều hướng theo vai trò
            // Điều hướng theo vai trò
            if (user.MaVaiTro == 1)
            {
                // Admin
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else if (user.MaVaiTro == 2)
            {
                // Giảng viên → tìm mã GV và chuyển sang trang "Lớp học phần của tôi"
                var giangVien = await _context.GiaoViens
                    .FirstOrDefaultAsync(g => g.MaGv == user.MaGv);

                if (giangVien != null)
                    return RedirectToAction("Index", "Home", new { area = "Giangvien", maGV = giangVien.MaGv });
                else
                    return RedirectToAction("Index", "Home", new { area = "Giangvien" });
            }


            else
            {
                // Sinh viên
                return RedirectToAction("Index", "Home", new { area = "" });
            }

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}

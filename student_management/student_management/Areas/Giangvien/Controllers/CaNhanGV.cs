using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;
using Microsoft.AspNetCore.Hosting;

namespace student_management.Areas.Giangvien.Controllers
{
    [Area("Giangvien")]
    public class CaNhanGVController : Controller
    {
        private readonly QuanlyhocDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CaNhanGVController(QuanlyhocDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ------------------------- HIỂN THỊ HỒ SƠ -------------------------
        public async Task<IActionResult> Index()
        {
            var tenDangNhap = User.Identity?.Name;
            if (tenDangNhap == null) return RedirectToAction("Login", "Account");

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.MaVaiTroNavigation)
                .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);

            if (taiKhoan == null || taiKhoan.MaGv == null)
                return RedirectToAction("KhongCoQuyen", "TaiKhoan");

            var gv = await _context.GiaoViens
                .Include(g => g.MaKhoaNavigation)
                .FirstOrDefaultAsync(g => g.MaGv == taiKhoan.MaGv);

            return View(gv);
        }

        // ------------------------- GET: CHỈNH SỬA HỒ SƠ -------------------------
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var tenDangNhap = User.Identity?.Name;
            if (tenDangNhap == null) return RedirectToAction("Login", "Account");

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);

            if (taiKhoan == null || taiKhoan.MaGv == null)
                return RedirectToAction("KhongCoQuyen", "TaiKhoan");

            var gv = await _context.GiaoViens
                .Include(g => g.MaKhoaNavigation)
                .FirstOrDefaultAsync(g => g.MaGv == taiKhoan.MaGv);

            if (gv == null) return NotFound();

            return View(gv);
        }

        // ------------------------- POST: LƯU CHỈNH SỬA -------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GiaoVien model, IFormFile? AnhFile)
        {
            var gv = await _context.GiaoViens.FirstOrDefaultAsync(g => g.MaGv == model.MaGv);
            if (gv == null)
            {
                TempData["Error"] = "Không tìm thấy giảng viên cần cập nhật!";
                return RedirectToAction("Index");
            }

            // Cập nhật thông tin cơ bản
            gv.HoTen = model.HoTen;
            gv.Email = model.Email;
            gv.SoDienThoai = model.SoDienThoai;
            gv.NgaySinh = model.NgaySinh;

            // Xử lý ảnh đại diện
            if (AnhFile != null && AnhFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(AnhFile.FileName);
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AnhFile.CopyToAsync(stream);
                }

                gv.Anh = "/uploads/" + fileName;

            }

            _context.Update(gv);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction("Index");
        }

        // ------------------------- GET: ĐỔI MẬT KHẨU -------------------------
        [HttpGet]
        public IActionResult ChangePassword() => View();

        // ------------------------- POST: ĐỔI MẬT KHẨU -------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var tenDangNhap = User.Identity?.Name;
            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);

            if (taiKhoan == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản!";
                return View();
            }

            if (taiKhoan.MatKhau != oldPassword)
            {
                TempData["Error"] = "Mật khẩu cũ không đúng!";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu mới không khớp!";
                return View();
            }

            taiKhoan.MatKhau = newPassword;
            _context.Update(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;

namespace student_management.Controllers
{
    public class CaNhanSVController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public CaNhanSVController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (maSV == null)
                return RedirectToAction("Login", "Account");

            var sv = _context.SinhViens
                .Include(s => s.MaLopNavigation)
                .Include(s => s.MaKhoaNavigation)
                .FirstOrDefault(s => s.MaSv == maSV);

            return View(sv);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (maSV == null)
                return RedirectToAction("Login", "Account");

            var sv = _context.SinhViens
                .Include(s => s.MaLopNavigation)
                .Include(s => s.MaKhoaNavigation)
                .FirstOrDefault(s => s.MaSv == maSV);

            return View(sv);
        }

        public IActionResult Edit(string MaSv, string HoTen, DateTime NgaySinh, string Email, string SoDienThoai, IFormFile? AnhUpload)
        {
            var sv = _context.SinhViens.FirstOrDefault(x => x.MaSv == MaSv);
            if (sv == null) return NotFound();

            sv.HoTen = HoTen;
            sv.NgaySinh = DateOnly.FromDateTime(NgaySinh);
            sv.Email = Email;
            sv.SoDienThoai = SoDienThoai;

            if (AnhUpload != null && AnhUpload.Length > 0)
            {
                var fileName = $"{MaSv}_{AnhUpload.FileName}";
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/sinhvien", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    AnhUpload.CopyTo(stream);
                }

                sv.Anh = "/images/sinhvien/" + fileName;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (maSV == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoiMatKhau(string MatKhauCu, string MatKhauMoi, string XacNhanMatKhau)
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (maSV == null)
                return RedirectToAction("Login", "Account");

            if (MatKhauMoi != XacNhanMatKhau)
            {
                ViewBag.Error = "Xác nhận mật khẩu mới không khớp.";
                return View();
            }

            var tk = _context.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == maSV);
            if (tk == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản!";
                return View();
            }

            if (tk.MatKhau != MatKhauCu)
            {
                ViewBag.Error = "Mật khẩu cũ không đúng.";
                return View();
            }

            tk.MatKhau = MatKhauMoi;
            _context.SaveChanges();

            ViewBag.Success = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index");
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;
using student_management.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace student_management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuanLyTaiKhoanController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public QuanLyTaiKhoanController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GiangVien()
        {
            var gv = await _context.GiaoViens
                .Select(g => new QuanLyTaiKhoanViewModel
                {
                    MaNguoiDung = g.MaGv,
                    HoTen = g.HoTen,
                    VaiTro = "Giảng viên",
                    DaCoTaiKhoan = _context.TaiKhoans.Any(t => t.MaGv == g.MaGv)
                }).ToListAsync();

            return View(gv);
        }

        public async Task<IActionResult> SinhVien()
        {
            var sv = await _context.SinhViens
                .Select(s => new QuanLyTaiKhoanViewModel
                {
                    MaNguoiDung = s.MaSv,
                    HoTen = s.HoTen,
                    VaiTro = "Sinh viên",
                    DaCoTaiKhoan = _context.TaiKhoans.Any(t => t.MaSv == s.MaSv)
                }).ToListAsync();

            return View(sv);
        }

        [HttpPost]
        public async Task<IActionResult> Reset(string id, string role)
        {
            TaiKhoan? tk = null;

            if (role == "Giảng viên")
            {
                tk = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.MaGv == id);
                if (tk == null)
                    tk = new TaiKhoan { MaGv = id, TenDangNhap = id, MatKhau = "123456", MaVaiTro = 2, TrangThai = true, CreatedAt = DateTime.Now };
                else
                    tk.MatKhau = "123456";
            }
            else
            {
                tk = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.MaSv == id);
                if (tk == null)
                    tk = new TaiKhoan { MaSv = id, TenDangNhap = id, MatKhau = "123456", MaVaiTro = 3, TrangThai = true, CreatedAt = DateTime.Now };
                else
                    tk.MatKhau = "123456";
            }

            _context.Update(tk);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"✅ Đã reset mật khẩu {role} {id} về 123456!";
            return RedirectToAction(role == "Giảng viên" ? "GiangVien" : "SinhVien");
        }
    }

}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;
using System.Linq;
using System.Threading.Tasks;

namespace student_management.Areas.GiangVien.Controllers
{
    [Area("GiangVien")]
    [Authorize(Roles = "GiangVien")]
    public class QlyLopHocPhanController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public QlyLopHocPhanController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        // ✅ Danh sách lớp học phần mà giảng viên phụ trách
        public async Task<IActionResult> LopHocPhanCuaToi()
        {
            var maGv = User.Identity?.Name;

            var dsLHP = await _context.LopHocPhans
                .Include(l => l.MaMhNavigation)
                .Include(l => l.MaGvNavigation)
                .Include(l => l.DangKyHocs)
                .Where(l => l.MaGv == maGv)
                .ToListAsync();

            return View(dsLHP);
        }

        // ✅ Xem danh sách sinh viên chờ duyệt trong 1 lớp học phần
        public async Task<IActionResult> DuyetDangKy(int maLHP)
        {
            var dsDangKy = await _context.DangKyHocs
                .Include(d => d.MaSvNavigation)
                .Where(d => d.MaLhp == maLHP && d.TrangThai == "Chờ duyệt")
                .ToListAsync();

            ViewBag.MaLHP = maLHP;
            return View(dsDangKy);
        }

        // ✅ Duyệt sinh viên đăng ký
        [HttpPost]
        public async Task<IActionResult> XacNhanDuyet(int maLHP, string maSV)
        {
            var dk = await _context.DangKyHocs
                .FirstOrDefaultAsync(d => d.MaLhp == maLHP && d.MaSv == maSV);

            if (dk != null)
            {
                dk.TrangThai = "Đã duyệt";
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = $"✅ Đã duyệt sinh viên {maSV}";
            }

            return RedirectToAction("DuyetDangKy", new { maLHP });
        }

        // ✅ Từ chối sinh viên
        [HttpPost]
        public async Task<IActionResult> XoaDangKy(int maLHP, string maSV)
        {
            var dk = await _context.DangKyHocs
                .FirstOrDefaultAsync(d => d.MaLhp == maLHP && d.MaSv == maSV);

            if (dk != null)
            {
                _context.DangKyHocs.Remove(dk);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = $"❌ Đã từ chối sinh viên {maSV}";
            }

            return RedirectToAction("DuyetDangKy", new { maLHP });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;
using student_management.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;

namespace student_management.Controllers
{
    [Authorize(Roles = "SinhVien")] // ✅ chỉ cho sinh viên truy cập
    public class SinhVienController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public SinhVienController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        // ✅ 1. Danh sách lớp học phần sinh viên đang học
        public async Task<IActionResult> LopHocPhanCuaToi()
        {
            var maSV = HttpContext.Session.GetString("MaSV");

            if (string.IsNullOrEmpty(maSV))
                return RedirectToAction("Login", "Account");

            var lopHocPhans = await _context.DangKyHocs
                .Include(dk => dk.MaLhpNavigation)
                    .ThenInclude(l => l.MaMhNavigation) // Môn học
                .Include(dk => dk.MaLhpNavigation)
                    .ThenInclude(l => l.MaGvNavigation) // Giảng viên
                .Where(dk => dk.MaSv == maSV)
                .Select(dk => dk.MaLhpNavigation)
                .ToListAsync();

            return View(lopHocPhans);
        }

        // ✅ 2. Xem điểm của sinh viên trong tất cả lớp học phần
        public async Task<IActionResult> XemDiem()
        {
            var maSV = HttpContext.Session.GetString("MaSV");

            if (string.IsNullOrEmpty(maSV))
                return RedirectToAction("Login", "Account");

            var bangDiems = await _context.BangDiems
                .Include(b => b.DangKyHoc)
                    .ThenInclude(dk => dk.MaLhpNavigation)
                        .ThenInclude(lhp => lhp.MaMhNavigation)
                .Where(b => b.MaSv == maSV)
                .ToListAsync();

            if (bangDiems == null || !bangDiems.Any())
            {
                ViewBag.ThongBao = "Hiện tại bạn chưa có điểm được nhập.";
                return View(new List<BangDiem>());
            }

            return View(bangDiems);
        }

        // 📅 Xem lịch sử điểm danh
        public async Task<IActionResult> LichSuDiemDanh(int maLHP)
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV))
                return RedirectToAction("Login", "Account");

            // Lấy danh sách điểm danh của SV trong lớp học phần
            var danhSach = await _context.DiemDanhs
                .Where(d => d.MaLhp == maLHP && d.MaSv == maSV)
                .OrderByDescending(d => d.NgayDiemDanh)
                .ToListAsync();

            // Lấy tên lớp học phần
            var tenLHP = await _context.LopHocPhans
                .Where(l => l.MaLhp == maLHP)
                .Select(l => l.TenLhp)
                .FirstOrDefaultAsync();

            ViewBag.TenLHP = tenLHP;
            ViewBag.MaLHP = maLHP;

            // Debug xem có bản ghi nào không
            System.Diagnostics.Debug.WriteLine($"DEBUG => Found {danhSach.Count} bản ghi điểm danh cho SV: {maSV}, LHP: {maLHP}");
            foreach (var item in danhSach)
            {
                System.Diagnostics.Debug.WriteLine($"--> {item.MaSv} | {item.MaLhp} | {item.NgayDiemDanh:dd/MM/yyyy} | {item.TrangThai}");
            }

            return View(danhSach);
        }

    }
}

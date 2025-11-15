using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;
using student_management.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace student_management.Controllers
{
    [Authorize(Roles = "SinhVien")]
    public class ThoiKhoaBieuController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public ThoiKhoaBieuController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV))
                return RedirectToAction("Login", "Account");

            // 1. Lấy tất cả lớp học phần đã đăng ký và duyệt
            var dkList = await _context.DangKyHocs
                .Where(dk => dk.MaSv == maSV && dk.TrangThai == "Đã duyệt")
                .Include(dk => dk.MaLhpNavigation)
                    .ThenInclude(l => l.MaMhNavigation)
                .Include(dk => dk.MaLhpNavigation)
                    .ThenInclude(l => l.MaGvNavigation)
                .Include(dk => dk.MaLhpNavigation)
                    .ThenInclude(l => l.ThoiKhoaBieus)
                .ToListAsync();

            // 2. Map sang ViewModel (sử dụng LINQ to Objects để tránh ?. trong LINQ to Entities)
            var tkbList = dkList
                .SelectMany(dk => dk.MaLhpNavigation.ThoiKhoaBieus.DefaultIfEmpty(), (dk, tkb) => new ThoiKhoaBieuViewModel
                {
                    TenMonHoc = dk.MaLhpNavigation.MaMhNavigation.TenMh,
                    TenLopHocPhan = dk.MaLhpNavigation.TenLhp,
                    GiangVien = dk.MaLhpNavigation.MaGvNavigation?.HoTen ?? "Chưa có",
                    Thu = tkb != null ? tkb.Thu : 0,
                    TietBatDau = tkb != null ? tkb.TietBatDau : 0,
                    SoTiet = tkb != null ? tkb.SoTiet : 0,
                    PhongHoc = tkb?.PhongHoc ?? "Chưa xác định",
                    HocKy = dk.MaLhpNavigation.HocKy,
                    NamHoc = dk.MaLhpNavigation.NamHoc
                })
                .OrderBy(t => t.Thu)
                .ThenBy(t => t.TietBatDau)
                .ToList();

            return View(tkbList);
        }
    }
}

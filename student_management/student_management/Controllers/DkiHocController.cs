using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;
using student_management.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace student_management.Controllers
{
    [Authorize(Roles = "SinhVien")]
    public class DkiHocController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public DkiHocController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var maSv = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSv))
                return RedirectToAction("Login", "Account");

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaKhoaNavigation)
                .FirstOrDefaultAsync(s => s.MaSv == maSv);

            if (sinhVien == null)
                return NotFound("Không tìm thấy sinh viên.");

            var lopHocPhans = await _context.LopHocPhans
                .Include(l => l.MaMhNavigation)
                .Include(l => l.MaGvNavigation)
                .Where(l => l.MaMhNavigation.MaKhoa == sinhVien.MaKhoa)
                .ToListAsync();

            var dangKyList = await _context.DangKyHocs
                .Where(d => d.MaSv == maSv)
                .ToListAsync();

            var bangDiemList = await _context.BangDiems
                .Where(b => b.MaSv == maSv)
                .ToListAsync();

            var vm = new DkiHocIndexViewModel
            {
                TenKhoa = sinhVien.MaKhoaNavigation?.TenKhoa ?? "Không xác định"
            };

            foreach (var lhp in lopHocPhans)
            {
                var dk = dangKyList.FirstOrDefault(d => d.MaLhp == lhp.MaLhp);
                var diem = bangDiemList.FirstOrDefault(b => b.MaLhp == lhp.MaLhp);

                var lopVm = new LopHocPhanViewModel
                {
                    MaLhp = lhp.MaLhp,
                    TenLhp = lhp.TenLhp,
                    GiangVien = lhp.MaGvNavigation?.HoTen ?? "",
                    MaMh = lhp.MaMh,
                    TenMonHoc = lhp.MaMhNavigation?.TenMh ?? "",
                    SoTinChi = lhp.MaMhNavigation?.SoTinChi ?? 0,
                    SiSoToiDa = lhp.SiSoToiDa,
                    SiSoHienTai = _context.DangKyHocs.Count(d => d.MaLhp == lhp.MaLhp),
                    TrangThaiDangKy = dk?.TrangThai ?? "Chưa đăng ký",
                    DiemTongKet = diem?.DiemTongKet
                };

                if (dk == null || dk.TrangThai?.Trim().ToLower() != "đã duyệt" || (diem != null && diem.DiemTongKet < 4))
                    vm.LopChuaHoc.Add(lopVm);
                else if (lhp.TrangThai == "Đang mở" && dk.TrangThai?.Trim().ToLower() == "đã duyệt")
                    vm.LopDangHoc.Add(lopVm);
                else if (lhp.TrangThai == "Đã kết thúc" && diem != null && diem.DiemTongKet >= 5)
                    vm.LopHoanThanh.Add(lopVm);
            }

            return View(vm);
        }
    }
}

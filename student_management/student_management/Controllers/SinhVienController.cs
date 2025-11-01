using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;
using student_management.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace student_management.Controllers
{
    [Authorize(Roles = "SinhVien")]
    public class SinhVienController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public SinhVienController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        private string ChuyenTrangThai(string trangThai)
        {
            if (string.IsNullOrEmpty(trangThai)) return "Chưa đăng ký";
            trangThai = trangThai.Trim().ToLower();
            if (trangThai.Contains("xóa") || trangThai.Contains("hủy") || trangThai.Contains("huy") || trangThai.Contains("xoa"))
                return "Chưa đăng ký";
            else if (trangThai.Contains("chờ") || trangThai.Contains("cho"))
                return "Chờ duyệt";
            else if (trangThai.Contains("duyệt"))
                return "Đã duyệt";
            else
                return trangThai;
        }

        // 1. Danh sách lớp học phần sinh viên đang học
        // Danh sách lớp học phần sinh viên đang học (chỉ hiển thị lớp đã đăng ký)
        public async Task<IActionResult> LopHocPhanCuaToi()
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV)) return RedirectToAction("Login", "Account");

            var lopHocPhans = await _context.DangKyHocs
                .Include(dk => dk.MaLhpNavigation)
                    .ThenInclude(l => l.MaMhNavigation)
                .Include(dk => dk.MaLhpNavigation)
                    .ThenInclude(l => l.MaGvNavigation)
                .Where(dk => dk.MaSv == maSV &&
                            (dk.TrangThai.Trim().ToLower() == "đã duyệt" || dk.TrangThai.Trim().ToLower() == "da duyet" ||
                             dk.TrangThai.Trim().ToLower() == "chờ duyệt" || dk.TrangThai.Trim().ToLower() == "cho duyệt"))
                .Select(dk => dk.MaLhpNavigation)
                .ToListAsync();

            return View(lopHocPhans);
        }
        // 2. Xem điểm
        public async Task<IActionResult> XemDiem()
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV)) return RedirectToAction("Login", "Account");

            var bangDiems = await _context.BangDiems
                .Include(b => b.DangKyHoc)
                    .ThenInclude(dk => dk.MaLhpNavigation)
                        .ThenInclude(lhp => lhp.MaMhNavigation)
                .Where(b => b.MaSv == maSV)
                .ToListAsync();

            if (!bangDiems.Any())
            {
                ViewBag.ThongBao = "Hiện tại bạn chưa có điểm được nhập.";
                return View(new List<BangDiem>());
            }

            return View(bangDiems);
        }

        // 3. Lịch sử điểm danh
        public async Task<IActionResult> LichSuDiemDanh(int maLHP)
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV)) return RedirectToAction("Login", "Account");

            var danhSach = await _context.DiemDanhs
                .Where(d => d.MaLhp == maLHP && d.MaSv == maSV)
                .OrderByDescending(d => d.NgayDiemDanh)
                .ToListAsync();

            var tenLHP = await _context.LopHocPhans
                .Where(l => l.MaLhp == maLHP)
                .Select(l => l.TenLhp)
                .FirstOrDefaultAsync();

            ViewBag.TenLHP = tenLHP;
            ViewBag.MaLHP = maLHP;

            return View(danhSach);
        }

        // 4. Học phần CNTT
        public IActionResult HocPhanCNTT()
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV)) return RedirectToAction("Login", "Account");

            var sinhVien = _context.SinhViens
                .Include(sv => sv.DangKyHocs)
                    .ThenInclude(dk => dk.MaLhpNavigation)
                .FirstOrDefault(sv => sv.MaSv == maSV);

            if (sinhVien == null) return NotFound();

            var tatCaMonCNTT = _context.MonHocs
                .Include(m => m.LopHocPhans)
                .Where(m => m.MaKhoa == "CNTT")
                .ToList();

            var monDaHoc = sinhVien.DangKyHocs
                .Where(dk => dk.TrangThai.Trim().ToLower() == "đã duyệt" || dk.TrangThai.Trim().ToLower() == "da duyet")
                .Select(dk => dk.MaLhpNavigation.MaMh)
                .Distinct()
                .ToList();

            var viewModel = tatCaMonCNTT.Select(m => new MonHocHoanThanhViewModel
            {
                MaMh = m.MaMh,
                TenMh = m.TenMh,
                SoTinChi = m.SoTinChi,
                HoanThanh = monDaHoc.Contains(m.MaMh),
                LopDangMo = m.LopHocPhans?.Count(l =>
                    l.TrangThai.Trim().ToLower() == "đang mở" ||
                    l.TrangThai.Trim().ToLower() == "dang mo" ||
                    l.TrangThai.Trim().ToLower() == "chờ duyệt" ||
                    l.TrangThai.Trim().ToLower() == "cho duyet") ?? 0
            }).ToList();

            return View(viewModel);
        }

        // 5. Chi tiết môn học
        // Xem chi tiết lớp học phần của môn học
        public async Task<IActionResult> ChiTietMon(string maMh)
        {
            if (string.IsNullOrEmpty(maMh)) return NotFound();
            var maSV = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV)) return RedirectToAction("Login", "Account");

            var lopHocPhans = await _context.LopHocPhans
                .Include(l => l.MaGvNavigation)
                .Include(l => l.MaMhNavigation)
                .Include(l => l.DangKyHocs)
                .Where(l => l.MaMh == maMh)
                .ToListAsync();

            var viewModel = new List<LopHocPhanViewModel>();

            foreach (var l in lopHocPhans)
            {
                // Lấy trạng thái đăng ký của SV
                var dk = l.DangKyHocs.FirstOrDefault(x => x.MaSv == maSV);
                string trangThaiDK = ChuyenTrangThai(dk?.TrangThai);

                // Tính sĩ số hiện tại (số SV đã duyệt)
                int siSoHienTai = await _context.DangKyHocs
                    .CountAsync(d => d.MaLhp == l.MaLhp &&
                                     (d.TrangThai.Trim().ToLower() == "đã duyệt" || d.TrangThai.Trim().ToLower() == "da duyet"));

                viewModel.Add(new LopHocPhanViewModel
                {
                    MaLhp = l.MaLhp,
                    TenLhp = l.TenLhp,
                    GiangVien = l.MaGvNavigation?.HoTen ?? "Chưa có",
                    SiSoToiDa = l.SiSoToiDa,
                    SiSoHienTai = siSoHienTai,
                    TrangThai = l.TrangThai,
                    TrangThaiDangKy = trangThaiDK
                });
            }

            ViewBag.TenMon = lopHocPhans.FirstOrDefault()?.MaMhNavigation?.TenMh
                             ?? await _context.MonHocs
                                    .Where(m => m.MaMh == maMh)
                                    .Select(m => m.TenMh)
                                    .FirstOrDefaultAsync();

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> DangKyHocPhan(int maLHP)
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV)) return RedirectToAction("Login", "Account");

            var lop = await _context.LopHocPhans.FirstOrDefaultAsync(l => l.MaLhp == maLHP);
            if (lop == null)
            {
                TempData["ThongBao"] = "Lớp học phần không tồn tại!";
                return RedirectToAction("HocPhanCNTT");
            }

            // Kiểm tra sĩ số
            int siSoHienTai = await _context.DangKyHocs
                .CountAsync(d => d.MaLhp == maLHP &&
                                 (d.TrangThai.Trim().ToLower() == "đã duyệt" || d.TrangThai.Trim().ToLower() == "da duyet"));

            if (siSoHienTai >= lop.SiSoToiDa)
            {
                TempData["ThongBao"] = "Lớp học phần đã đủ sĩ số!";
                return RedirectToAction("ChiTietMon", new { maMh = lop.MaMh });
            }

            // Kiểm tra SV đã đăng ký chưa
            bool daDangKy = await _context.DangKyHocs.AnyAsync(d => d.MaSv == maSV && d.MaLhp == maLHP);
            if (daDangKy)
            {
                TempData["ThongBao"] = "Bạn đã đăng ký lớp học phần này!";
                return RedirectToAction("ChiTietMon", new { maMh = lop.MaMh });
            }

            var dangKy = new DangKyHoc
            {
                MaSv = maSV,
                MaLhp = maLHP,
                NgayDangKy = DateTime.Now,
                TrangThai = "Chờ duyệt"
            };
            _context.DangKyHocs.Add(dangKy);

            // Tạo thông báo cho giảng viên
            if (!string.IsNullOrEmpty(lop.MaGv))
            {
                _context.ThongBaos.Add(new ThongBao
                {
                    TieuDe = "Yêu cầu đăng ký lớp học phần",
                    NoiDung = $"Sinh viên {maSV} đăng ký lớp {lop.TenLhp}. Vui lòng duyệt.",
                    NgayDang = DateTime.Now,
                    MaGv = lop.MaGv,
                    MaLhp = maLHP
                });
            }

            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Đăng ký thành công! Chờ giảng viên duyệt.";
            return RedirectToAction("ChiTietMon", new { maMh = lop.MaMh });
        }
        [HttpPost]
        public async Task<IActionResult> HuyDangKyHocPhan(int maLHP)
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV)) return RedirectToAction("Login", "Account");

            var dk = await _context.DangKyHocs
                .FirstOrDefaultAsync(d => d.MaSv == maSV && d.MaLhp == maLHP);

            if (dk != null)
            {
                _context.DangKyHocs.Remove(dk);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = "Hủy đăng ký thành công!";
            }

            var lop = await _context.LopHocPhans.FirstOrDefaultAsync(l => l.MaLhp == maLHP);
            return RedirectToAction("ChiTietMon", new { maMh = lop?.MaMh });
        }
    }
}

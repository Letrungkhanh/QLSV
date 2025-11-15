using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;
using student_management.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using student_management.Helpers;

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
            if (string.IsNullOrEmpty(trangThai))
                return TrangThaiDangKy.ChuaDangKy;

            trangThai = trangThai.Trim().ToLower();

            if (trangThai.Contains("xóa") || trangThai.Contains("hủy") || trangThai.Contains("huy") || trangThai.Contains("xoa"))
                return TrangThaiDangKy.DaXoa;
            else if (trangThai.Contains("chờ") || trangThai.Contains("cho"))
                return TrangThaiDangKy.ChoDuyet;
            else if (trangThai.Contains("duyệt"))
                return TrangThaiDangKy.DaDuyet;
            else
                return TrangThaiDangKy.ChuaDangKy; // dùng hằng số thay vì trả về chuỗi gốc
        }

        // 1. Danh sách lớp học phần sinh viên đang học
        // Danh sách lớp học phần sinh viên đang học (chỉ hiển thị lớp đã đăng ký)
        // [Authorize(Roles = "SinhVien")]
        public async Task<IActionResult> LopHocPhanCuaToi()
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV))
                return RedirectToAction("Login", "Account");

            var lopDangHoc = await _context.DangKyHocs
                .Where(dk => dk.MaSv == maSV &&
                             (dk.TrangThai == "Chờ duyệt" || dk.TrangThai == "Đã duyệt") &&
                             dk.MaLhpNavigation.TrangThai != "Đã kết thúc") // ✅ Ẩn lớp đã kết thúc
                .Include(dk => dk.MaLhpNavigation)
                    .ThenInclude(l => l.MaMhNavigation)
                .Include(dk => dk.MaLhpNavigation)
                    .ThenInclude(l => l.MaGvNavigation)
                .Select(dk => new LopHocPhanViewModel
                {
                    MaLhp = dk.MaLhp,
                    TenLhp = dk.MaLhpNavigation.TenLhp,
                    GiangVien = dk.MaLhpNavigation.MaGvNavigation.HoTen,
                    MaMh = dk.MaLhpNavigation.MaMh,
                    TenMonHoc = dk.MaLhpNavigation.MaMhNavigation.TenMh,
                    SoTinChi = dk.MaLhpNavigation.MaMhNavigation.SoTinChi,
                    SiSoToiDa = dk.MaLhpNavigation.SiSoToiDa,
                    SiSoHienTai = _context.DangKyHocs.Count(d => d.MaLhp == dk.MaLhp),
                    TrangThaiDangKy = dk.TrangThai
                })
                .ToListAsync();

            return View(lopDangHoc);
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
        public async Task<IActionResult> HocPhanCNTT()
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV))
                return RedirectToAction("Login", "Account");

            // Lấy danh sách mã môn mà SV đã đăng ký
            var monDaDangKy = await _context.DangKyHocs
                .Where(x => x.MaSv == maSV)
                .Select(x => x.MaLhpNavigation.MaMh)
                .Distinct()
                .ToListAsync();

            // Lấy danh sách môn CNTT chưa đăng ký
            var model = await _context.MonHocs
                .Where(mh => mh.MaKhoa == "CNTT" && !monDaDangKy.Contains(mh.MaMh))
                .Select(mh => new LopHocPhanViewModel
                {
                    MaMh = mh.MaMh,
                    TenMonHoc = mh.TenMh,
                    SoTinChi = mh.SoTinChi,

                    // lấy lớp đang mở nếu có
                    MaLhp = _context.LopHocPhans
                        .Where(l => l.MaMh == mh.MaMh && l.TrangThai == "Đang mở")
                        .Select(l => l.MaLhp)
                        .FirstOrDefault(),

                    TenLhp = _context.LopHocPhans
                        .Where(l => l.MaMh == mh.MaMh && l.TrangThai == "Đang mở")
                        .Select(l => l.TenLhp)
                        .FirstOrDefault(),

                    GiangVien = _context.LopHocPhans
                        .Where(l => l.MaMh == mh.MaMh && l.TrangThai == "Đang mở" && l.MaGvNavigation != null)
                        .Select(l => l.MaGvNavigation.HoTen)
                        .FirstOrDefault() ?? "Chưa có",

                    SiSoHienTai = _context.LopHocPhans
                        .Where(l => l.MaMh == mh.MaMh && l.TrangThai == "Đang mở")
                        .Select(l => l.SiSoHienTai)
                        .FirstOrDefault(),

                    SiSoToiDa = _context.LopHocPhans
                        .Where(l => l.MaMh == mh.MaMh && l.TrangThai == "Đang mở")
                        .Select(l => l.SiSoToiDa)
                        .FirstOrDefault(),

                    TrangThai = _context.LopHocPhans
                        .Where(l => l.MaMh == mh.MaMh && l.TrangThai == "Đang mở")
                        .Select(l => l.TrangThai)
                        .FirstOrDefault() ?? "Chưa mở",

                    TrangThaiDangKy = "Chưa đăng ký"
                })
                .ToListAsync();

            return View(model);
        }




        public IActionResult MonHocHoanThanh()
        {
            var maSV = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV)) return RedirectToAction("Login", "Account");

            var sinhVien = _context.SinhViens
                .Include(sv => sv.DangKyHocs)
                    .ThenInclude(dk => dk.MaLhpNavigation)
                        .ThenInclude(lhp => lhp.MaMhNavigation)
                .Include(sv => sv.DangKyHocs)
                    .ThenInclude(dk => dk.BangDiem)
                .Include(sv => sv.DangKyHocs)
                    .ThenInclude(dk => dk.DiemDanhs)
                .FirstOrDefault(sv => sv.MaSv == maSV);

            if (sinhVien == null) return NotFound();

            var monHoanThanh = sinhVien.DangKyHocs
                .Where(dk => dk.KetQua == "Hoàn thành")
                .Select(dk =>
                {
                    int tongBuoi = dk.DiemDanhs.Count();
                    int vang = dk.DiemDanhs.Count(dd => dd.TrangThai == "Vắng");
                    bool vangQua30 = tongBuoi > 0 && vang > tongBuoi * 0.3;

                    decimal diemTongKet = dk.BangDiem?.DiemTongKet ?? 0;

                    return new MonHocHoanThanhViewModel
                    {
                        MaMh = dk.MaLhpNavigation.MaMh,
                        TenMh = dk.MaLhpNavigation.MaMhNavigation.TenMh,
                        SoTinChi = dk.MaLhpNavigation.MaMhNavigation.SoTinChi,
                        HoanThanh = diemTongKet >= 5 && !vangQua30,
                        Diem = diemTongKet,
                        VangQua30 = vangQua30,
                        LopHocPhanDaDangKy = new List<(int MaLhp, string TenLhp)>
                        {
                    (dk.MaLhpNavigation.MaLhp, dk.MaLhpNavigation.TenLhp)
                        }
                    };
                })
                .Where(x => x.HoanThanh)
                .GroupBy(x => x.MaMh)
                .Select(g =>
                {
                    var first = g.First();
                    return new MonHocHoanThanhViewModel
                    {
                        MaMh = g.Key,
                        TenMh = first.TenMh,
                        SoTinChi = first.SoTinChi,
                        HoanThanh = first.HoanThanh,
                        Diem = first.Diem,
                        VangQua30 = first.VangQua30,
                        LopHocPhanDaDangKy = g.SelectMany(x => x.LopHocPhanDaDangKy).ToList()
                    };
                })
                .ToList();

            return View(monHoanThanh);
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
                                 (d.TrangThai == TrangThaiDangKy.DaDuyet));

            if (siSoHienTai >= lop.SiSoToiDa)
            {
                TempData["ThongBao"] = "Lớp học phần đã đủ sĩ số!";
                return RedirectToAction("ChiTietMon", new { maMh = lop.MaMh });
            }

            // Kiểm tra xem sinh viên đã có bản đăng ký chưa
            var existingDK = await _context.DangKyHocs
                .FirstOrDefaultAsync(d => d.MaSv == maSV && d.MaLhp == maLHP);

            if (existingDK == null)
            {
                _context.DangKyHocs.Add(new DangKyHoc
                {
                    MaSv = maSV,
                    MaLhp = maLHP,
                    NgayDangKy = DateTime.Now,
                    TrangThai = TrangThaiDangKy.ChoDuyet
                });
            }
            else
            {
                if (existingDK.TrangThai == TrangThaiDangKy.DaXoa)
                {
                    existingDK.TrangThai = TrangThaiDangKy.ChoDuyet;
                    existingDK.NgayDangKy = DateTime.Now;
                    _context.DangKyHocs.Update(existingDK);
                }
                else
                {
                    TempData["ThongBao"] = "Bạn đã đăng ký lớp học phần này!";
                    return RedirectToAction("ChiTietMon", new { maMh = lop.MaMh });
                }
            }

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
                dk.TrangThai = "Đã rút";
                
                _context.DangKyHocs.Update(dk);

                // Cập nhật sĩ số lớp
                var lop = await _context.LopHocPhans.FirstOrDefaultAsync(l => l.MaLhp == maLHP);
                if (lop != null)
                {
                    lop.SiSoHienTai = await _context.DangKyHocs
                        .CountAsync(d => d.MaLhp == maLHP && d.TrangThai == TrangThaiDangKy.DaDuyet);
                    _context.LopHocPhans.Update(lop);
                }

                await _context.SaveChangesAsync();
                TempData["ThongBao"] = "Hủy đăng ký thành công!";
            }

            var lopMon = await _context.LopHocPhans.FirstOrDefaultAsync(l => l.MaLhp == maLHP);
            return RedirectToAction("ChiTietMon", new { maMh = lopMon?.MaMh });
        }
        public async Task<IActionResult> ThongBaoCuaToi(int maLhp)
        {
            var tenDangNhap = User.Identity?.Name;
            if (string.IsNullOrEmpty(tenDangNhap))
                return RedirectToAction("Login", "Account", new { area = "" });

            // Lấy tài khoản sinh viên
            var sinhVien = await _context.SinhViens
                .Include(sv => sv.DangKyHocs)
                .FirstOrDefaultAsync(sv => sv.TaiKhoans.Any(tk => tk.TenDangNhap == tenDangNhap));

            if (sinhVien == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            // Kiểm tra sinh viên đang học lớp này (trạng thái đã duyệt)
            var hocLop = sinhVien.DangKyHocs.Any(dk => dk.MaLhp == maLhp && dk.TrangThai == "Đã duyệt");
            if (!hocLop)
                return NotFound("Bạn không đăng ký lớp này hoặc chưa được duyệt.");

            // Lấy thông báo liên quan lớp đó
            var thongBaoList = await _context.ThongBaos
     .AsNoTracking()
     .Include(tb => tb.MaLhpNavigation)
     .Include(tb => tb.MaGvNavigation)
     .Where(tb => tb.MaLhp == maLhp)
     .OrderByDescending(tb => tb.NgayDang)
     .ToListAsync();


            return View(thongBaoList);
        }




    }
}

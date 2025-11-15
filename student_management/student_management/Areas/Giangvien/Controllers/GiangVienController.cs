using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;
using student_management.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using System;
using student_management.Helpers;

namespace student_management.Areas.GiangVien.Controllers
{
    [Area("GiangVien")]
    [Authorize(Roles = "GiangVien")]
    public class GiangVienController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public GiangVienController(QuanlyhocDbContext context)
        {
            _context = context;
        }
        private async Task CapNhatSiSoHienTai(int maLHP)
        {
            var lop = await _context.LopHocPhans.FirstOrDefaultAsync(l => l.MaLhp == maLHP);
            if (lop != null)
            {
                lop.SiSoHienTai = await _context.DangKyHocs
                    .CountAsync(d => d.MaLhp == maLHP && d.TrangThai == TrangThaiDangKy.DaDuyet);

                _context.LopHocPhans.Update(lop);
                await _context.SaveChangesAsync();
            }
        }


        // ✅ 1. Hiển thị danh sách lớp học phần của giảng viên
        public async Task<IActionResult> LopHocPhanCuaToi()
        {
            var tenDangNhap = User.Identity?.Name;
            if (string.IsNullOrEmpty(tenDangNhap))
                return RedirectToAction("Login", "Account", new { area = "" });

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);

            if (taiKhoan == null || string.IsNullOrEmpty(taiKhoan.MaGv))
                return RedirectToAction("Login", "Account", new { area = "" });

            var maGV = taiKhoan.MaGv;

            var lopHocPhans = await _context.LopHocPhans
                .Include(l => l.MaMhNavigation)
                .Include(l=>l.MaGvNavigation)
                .Include(l => l.DangKyHocs)
                .Where(l => l.MaGv == maGV)
                .ToListAsync();

            return View(lopHocPhans);
        }

        // ✅ 2. Danh sách sinh viên của lớp (chỉ hiện SV đã duyệt)
        public async Task<IActionResult> DanhSachSinhVien(int maLHP)
        {
            var lopHocPhan = await _context.LopHocPhans
                .FirstOrDefaultAsync(l => l.MaLhp == maLHP);

            if (lopHocPhan == null) return NotFound();

            var sinhViens = await _context.DangKyHocs
                .Where(d => d.MaLhp == maLHP &&( d.TrangThai == "Đã duyệt" ||d.TrangThai == "Đã rút" ))
                .Include(d => d.MaSvNavigation)
                .Select(d => new DiemDanhItem
                {
                    MaSV = d.MaSv,
                    HoTen = d.MaSvNavigation.HoTen,
                    TrangThai = d.TrangThai
                })
                .ToListAsync();

            var model = new DiemDanhViewModel
            {
                MaLHP = maLHP,
                TenLHP = lopHocPhan.TenLhp,
                SinhViens = sinhViens
            };

            return View(model);
        }

        // ✅ 3. Điểm danh (GET)
        public async Task<IActionResult> DiemDanh(int maLHP)
        {
            var lopHocPhan = await _context.LopHocPhans
                .Include(l => l.DangKyHocs.Where(d => d.TrangThai == "Đã duyệt"))
                .ThenInclude(d => d.MaSvNavigation)
                .FirstOrDefaultAsync(l => l.MaLhp == maLHP);

            if (lopHocPhan == null) return NotFound();

            var model = new DiemDanhViewModel
            {
                MaLHP = lopHocPhan.MaLhp,
                TenLHP = lopHocPhan.TenLhp,
                SinhViens = lopHocPhan.DangKyHocs.Select(d => new DiemDanhItem
                {
                    MaSV = d.MaSv,
                    HoTen = d.MaSvNavigation.HoTen
                }).ToList()
            };

            return View(model);
        }

        // ✅ 4. Điểm danh (POST)
        [HttpPost]
        public async Task<IActionResult> DiemDanh(DiemDanhViewModel model)
        {
            foreach (var item in model.SinhViens)
            {
                var existing = await _context.DiemDanhs
                    .FirstOrDefaultAsync(d => d.MaLhp == model.MaLHP
                                           && d.MaSv == item.MaSV
                                           && d.NgayDiemDanh == DateOnly.FromDateTime(DateTime.Now));

                if (existing != null)
                    existing.TrangThai = item.TrangThai;
                else
                    _context.DiemDanhs.Add(new DiemDanh
                    {
                        MaLhp = model.MaLHP,
                        MaSv = item.MaSV,
                        NgayDiemDanh = DateOnly.FromDateTime(DateTime.Now),
                        TrangThai = item.TrangThai
                    });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Đã lưu điểm danh!";
            return RedirectToAction(nameof(DiemDanh), new { maLHP = model.MaLHP });
        }

        // ✅ 5. Thêm sinh viên (chỉ thêm với trạng thái "Chờ duyệt")
        public async Task<IActionResult> ThemSinhVien(int maLHP)
        {
            ViewBag.MaLHP = maLHP;

            var dsSVChuaCo = await _context.SinhViens
                .Where(sv => !_context.DangKyHocs.Any(dk => dk.MaLhp == maLHP && dk.MaSv == sv.MaSv))
                .ToListAsync();

            var model = new DanhSachChonSVViewModel
            {
                MaLHP = maLHP,
                SinhViens = dsSVChuaCo.Select(s => new SinhVienChonItem
                {
                    MaSV = s.MaSv,
                    HoTen = s.HoTen
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemSinhVien(DanhSachChonSVViewModel model)
        {
            var dsChon = model.SinhViens.Where(s => s.DaChon).ToList();
            foreach (var sv in dsChon)
            {
                if (!_context.DangKyHocs.Any(d => d.MaLhp == model.MaLHP && d.MaSv == sv.MaSV))
                {
                    _context.DangKyHocs.Add(new DangKyHoc
                    {
                        MaLhp = model.MaLHP,
                        MaSv = sv.MaSV,
                        NgayDangKy = DateTime.Now,
                        TrangThai = "Đã duyệt" // ✅ THÊM THẲNG VÀO LỚP, KHÔNG CHỜ DUYỆT
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Đã thêm sinh viên vào lớp.";

            // ✅ Chuyển về danh sách sinh viên của lớp thay vì trang duyệt
            return RedirectToAction("DanhSachSinhVien", "GiangVien", new { maLHP = model.MaLHP, area = "GiangVien" });
        }

        // ✅ 6. Duyệt sinh viên
        public async Task<IActionResult> DuyetDangKy(int maLHP)
            {
            var danhSachChoDuyet = await _context.DangKyHocs
                 .Include(dk => dk.MaSvNavigation)
                 .Where(dk => dk.MaLhp == maLHP &&
                          (EF.Functions.Like(dk.TrangThai.Trim().ToLower(), "%chờ%") ||
                           EF.Functions.Like(dk.TrangThai.Trim().ToLower(), "%cho%")))
                 .ToListAsync();

                 ViewBag.MaLHP = maLHP;
                return View(danhSachChoDuyet);
            }

        [HttpPost]
        public async Task<IActionResult> XacNhanDuyet(int maLHP, string maSV)
        {
            if (string.IsNullOrEmpty(maSV))
                return NotFound();

            var dk = await _context.DangKyHocs
                .FirstOrDefaultAsync(x => x.MaLhp == maLHP && x.MaSv == maSV);

            if (dk == null)
                return NotFound();

            dk.TrangThai = TrangThaiDangKy.DaDuyet;
            _context.DangKyHocs.Update(dk);

            await CapNhatSiSoHienTai(maLHP);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = $"✅ Đã duyệt sinh viên {maSV} vào lớp {maLHP}.";
            return RedirectToAction("DuyetDangKy", new { maLHP });
        }

        [HttpPost]
        public async Task<IActionResult> XoaDangKy(int maLHP, string maSV)
        {
            if (string.IsNullOrEmpty(maSV))
                return NotFound();

            var dk = await _context.DangKyHocs
                .FirstOrDefaultAsync(x => x.MaLhp == maLHP && x.MaSv == maSV);

            if (dk == null)
                return NotFound();

            dk.TrangThai = TrangThaiDangKy.DaXoa;
            _context.DangKyHocs.Update(dk);

            await CapNhatSiSoHienTai(maLHP);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = $"🗑️ Đã xóa sinh viên {maSV} khỏi lớp {maLHP}.";
            return RedirectToAction("DuyetDangKy", new { maLHP });
        }


        // ✅ 8. Nhập điểm
        public async Task<IActionResult> NhapDiem(int maLHP)
        {
            var lop = await _context.LopHocPhans
                .Include(l => l.DangKyHocs.Where(d => d.TrangThai == "Đã duyệt"))
                .ThenInclude(d => d.MaSvNavigation)
                .Include(l => l.DangKyHocs)
                .ThenInclude(d => d.BangDiem)
                .FirstOrDefaultAsync(l => l.MaLhp == maLHP);

            if (lop == null) return NotFound();

            var model = new NhapDiemViewModel
            {
                MaLHP = maLHP,
                TenLHP = lop.TenLhp,
                SinhViens = lop.DangKyHocs.Select(d => new SinhVienDiemItem
                {
                    MaSV = d.MaSv,
                    HoTen = d.MaSvNavigation.HoTen,
                    DiemChuyenCan = (double?)(d.BangDiem?.DiemChuyenCan ?? 0),
                    DiemGiuaKy = (double?)(d.BangDiem?.DiemGiuaKy ?? 0),
                    DiemCuoiKy = (double?)(d.BangDiem?.DiemCuoiKy ?? 0),
                    DiemTongKet = (double?)(d.BangDiem?.DiemTongKet ?? 0)
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuuDiem(NhapDiemViewModel model)
        {
            foreach (var sv in model.SinhViens)
            {
                var dk = await _context.DangKyHocs
                    .Include(d => d.BangDiem)
                    .FirstOrDefaultAsync(d => d.MaLhp == model.MaLHP && d.MaSv == sv.MaSV && d.TrangThai == "Đã duyệt");

                if (dk == null) continue;

                decimal cc = (decimal)(sv.DiemChuyenCan ?? 0);
                decimal gk = (decimal)(sv.DiemGiuaKy ?? 0);
                decimal ck = (decimal)(sv.DiemCuoiKy ?? 0);
                decimal tongKet = Math.Round(cc * 0.1m + gk * 0.3m + ck * 0.6m, 2);

                if (dk.BangDiem == null)
                {
                    dk.BangDiem = new BangDiem
                    {
                        MaSv = sv.MaSV,
                        MaLhp = model.MaLHP,
                        DiemChuyenCan = cc,
                        DiemGiuaKy = gk,
                        DiemCuoiKy = ck,
                        DiemTongKet = tongKet
                    };
                }
                else
                {
                    dk.BangDiem.DiemChuyenCan = cc;
                    dk.BangDiem.DiemGiuaKy = gk;
                    dk.BangDiem.DiemCuoiKy = ck;
                    dk.BangDiem.DiemTongKet = tongKet;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Đã lưu điểm thành công!";
            return RedirectToAction("DanhSachSinhVien", new { maLHP = model.MaLHP });
        }

        // ✅ 9. Thống kê điểm danh
        public async Task<IActionResult> ThongKeDiemDanh(int maLHP)
        {
            var thongKe = await _context.DiemDanhs
                .Where(d => d.MaLhp == maLHP)
                .GroupBy(d => d.MaSv)
                .Select(g => new ThongKeDiemDanhItem
                {
                    MaSV = g.Key,
                    HoTen = g.First().DangKyHoc.MaSvNavigation.HoTen,
                    SoBuoiCoMat = g.Count(x => x.TrangThai == "Có mặt"),
                    SoBuoiVang = g.Count(x => x.TrangThai == "Vắng")
                })
                .ToListAsync();

            var model = new ThongKeDiemDanhViewModel
            {
                MaLHP = maLHP,
                DanhSach = thongKe
            };

            return View(model);
        }
        // ✅ Hiển thị danh sách lớp có sinh viên chờ duyệt
        public async Task<IActionResult> DanhSachChoDuyet()
        {
            var tenDangNhap = User.Identity?.Name;
            if (string.IsNullOrEmpty(tenDangNhap))
                return RedirectToAction("Login", "Account", new { area = "" });

            // 🔹 Tìm giảng viên hiện tại
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);

            if (taiKhoan == null || string.IsNullOrEmpty(taiKhoan.MaGv))
                return RedirectToAction("Login", "Account", new { area = "" });

            var maGV = taiKhoan.MaGv;

            // 🔹 Tìm các lớp có sinh viên đang chờ duyệt
            var danhSachLopChoDuyet = await _context.LopHocPhans
                .Include(l => l.MaMhNavigation)
                .Include(l => l.DangKyHocs)
                .Where(d => EF.Functions.Like(d.TrangThai, "%chờ%") || EF.Functions.Like(d.TrangThai, "%cho%"))
                .ToListAsync();


            return View(danhSachLopChoDuyet);
        }
        [HttpGet]
        public async Task<IActionResult> KetThucHocPhan(int maLHP)
        {
            // Lấy lớp học phần
            var lopHocPhan = await _context.LopHocPhans
                .FirstOrDefaultAsync(l => l.MaLhp == maLHP);

            if (lopHocPhan == null)
                return NotFound();

            // Lấy danh sách sinh viên đã duyệt
            var danhSach = await _context.DangKyHocs
                .Where(d => d.MaLhp == maLHP && d.TrangThai == "Đã duyệt")
                .Include(d => d.BangDiem)      // Bảng điểm
                .Include(d => d.DiemDanhs)     // Danh sách điểm danh
                .ToListAsync();

            foreach (var dk in danhSach)
            {
                // Tính tổng buổi và số buổi vắng
                int tongBuoiHoc = dk.DiemDanhs.Count;
                int soBuoiVang = dk.DiemDanhs.Count(dd => dd.TrangThai == "Vắng");

                // Điều kiện vắng quá 30%
                bool vangQua30 = tongBuoiHoc > 0 && soBuoiVang > (tongBuoiHoc * 0.3);

                // Lấy điểm tổng kết (nếu null thì gán 0)
                decimal diem = dk.BangDiem?.DiemTongKet ?? 0;

                // Xét kết quả
                if (diem >= 5m && vangQua30 == false)
                    dk.KetQua = "Hoàn thành";
                else
                    dk.KetQua = "Không đạt";
            }

            // Cập nhật trạng thái lớp học phần
            lopHocPhan.TrangThai = "Đã kết thúc";

            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã kết thúc lớp học phần và xét kết quả cho sinh viên.";
            return RedirectToAction(nameof(LopHocPhanCuaToi));
        }
        public async Task<IActionResult> LichDayGv()
        {
            var tenDangNhap = User.Identity?.Name;
            if (string.IsNullOrEmpty(tenDangNhap))
                return RedirectToAction("Login", "Account", new { area = "" });

            // Lấy tài khoản GV
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.MaGvNavigation)
                .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);

            if (taiKhoan == null || string.IsNullOrEmpty(taiKhoan.MaGv))
                return RedirectToAction("Login", "Account", new { area = "" });

            var maGV = taiKhoan.MaGv;

            // Lấy lớp học phần
            var lopHocPhans = await _context.LopHocPhans
                .Where(l => l.MaGv == maGV)
                .Include(l => l.MaMhNavigation)
                .Include(l => l.ThoiKhoaBieus)
                .ToListAsync();

            // Map sang ViewModel, CHỐNG NULL 100%
            var tkbList = lopHocPhans
                .SelectMany(
                    lhp => lhp.ThoiKhoaBieus.DefaultIfEmpty(),
                    (lhp, tkb) => new ThoiKhoaBieuViewModel
                    {
                        TenMonHoc = lhp.MaMhNavigation?.TenMh ?? "(Chưa có môn học)",
                        TenLopHocPhan = lhp.TenLhp ?? "(Không có tên lớp)",
                        GiangVien = taiKhoan.MaGvNavigation?.HoTen ?? "(Không rõ GV)",

                        Thu = tkb?.Thu ?? 0,
                        TietBatDau = tkb?.TietBatDau ?? 0,
                        SoTiet = tkb?.SoTiet ?? 0,
                        PhongHoc = tkb?.PhongHoc ?? "(Chưa xác định)",

                        HocKy = lhp.HocKy,
                        NamHoc = lhp.NamHoc ?? "(Chưa rõ)"
                    })
                .OrderBy(t => t.Thu)
                .ThenBy(t => t.TietBatDau)
                .ToList();

            return View(tkbList);
        }
        public async Task<IActionResult> GuiThongBao(int maLHP)
        {
            var tenDangNhap = User.Identity?.Name;
            if (string.IsNullOrEmpty(tenDangNhap))
                return RedirectToAction("Login", "Account", new { area = "" });

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.MaGvNavigation)
                .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);

            if (taiKhoan == null || string.IsNullOrEmpty(taiKhoan.MaGv))
                return RedirectToAction("Login", "Account", new { area = "" });

            var lop = await _context.LopHocPhans
                .FirstOrDefaultAsync(l => l.MaLhp == maLHP && l.MaGv == taiKhoan.MaGv);

            if (lop == null) return NotFound();

            var model = new ThongBaoGuiViewModel
            {
                MaLHP = maLHP
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiThongBao(ThongBaoGuiViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var tenDangNhap = User.Identity?.Name;
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.MaGvNavigation)
                .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);

            if (taiKhoan == null) return Unauthorized();

            var thongBao = new ThongBao
            {
                MaLhp = model.MaLHP,
                MaGv = taiKhoan.MaGv,
                TieuDe = model.TieuDe,
                NoiDung = model.NoiDung,
                NgayDang = DateTime.Now
            };

            _context.ThongBaos.Add(thongBao);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã gửi thông báo cho lớp học phần.";
            return RedirectToAction("DanhSachSinhVien", new { maLHP = model.MaLHP });
        }


    }
}

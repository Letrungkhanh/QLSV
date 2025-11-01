using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;
using student_management.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using System;

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
                .Where(d => d.MaLhp == maLHP && d.TrangThai == "Đã duyệt")
                .Include(d => d.MaSvNavigation)
                .Select(d => new DiemDanhItem
                {
                    MaSV = d.MaSv,
                    HoTen = d.MaSvNavigation.HoTen
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
                        TrangThai = "Chờ duyệt"
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Đã gửi yêu cầu duyệt sinh viên.";
            return RedirectToAction("DuyetDangKy", new { maLHP = model.MaLHP });
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

            dk.TrangThai = "Đã duyệt";
            _context.DangKyHocs.Update(dk);

            var lop = await _context.LopHocPhans.FirstOrDefaultAsync(x => x.MaLhp == maLHP);
            if (lop != null)
            {
                lop.SiSoHienTai = await _context.DangKyHocs
                    .CountAsync(x => x.MaLhp == maLHP && x.TrangThai == "Đã duyệt");
                _context.LopHocPhans.Update(lop);
            }

            await _context.SaveChangesAsync();

            TempData["ThongBao"] = $"Đã duyệt sinh viên {maSV} vào lớp {maLHP}.";
            return RedirectToAction("DanhSachDangKy", new { maLHP });
        }


        [HttpPost]
        public async Task<IActionResult> XoaDangKy(string maLHP, string maSV)
        {
            if (string.IsNullOrEmpty(maLHP) || string.IsNullOrEmpty(maSV))
                return NotFound();

            int maLhpInt = int.Parse(maLHP); // ✅ ép kiểu

            var dk = await _context.DangKyHocs
                .FirstOrDefaultAsync(x => x.MaLhp == maLhpInt && x.MaSv == maSV);

            if (dk == null)
                return NotFound();

            dk.TrangThai = "Đã xóa";
            _context.DangKyHocs.Update(dk);

            var lop = await _context.LopHocPhans.FirstOrDefaultAsync(x => x.MaLhp == maLhpInt);
            if (lop != null)
            {
                lop.SiSoHienTai = _context.DangKyHocs.Count(x => x.MaLhp == maLhpInt && x.TrangThai == "Đã duyệt");
                _context.LopHocPhans.Update(lop);
            }

            await _context.SaveChangesAsync();
            TempData["ThongBao"] = $"Đã xóa sinh viên {maSV} khỏi lớp {maLHP}.";
            return RedirectToAction("DanhSachDangKy", new { maLHP });
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
    }
}

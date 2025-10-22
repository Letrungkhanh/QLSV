    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using student_management.Models;
    using student_management.Models.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
using student_management.Models.ViewModels;


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
        // ✅ Hiển thị danh sách lớp học phần của giảng viên đang đăng nhập
        public async Task<IActionResult> LopHocPhanCuaToi()
        {
            var tenDangNhap = User.Identity?.Name;

            if (string.IsNullOrEmpty(tenDangNhap))
                return RedirectToAction("Login", "Account", new { area = "" });

            // Tìm thông tin tài khoản của giảng viên
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);

            if (taiKhoan == null || string.IsNullOrEmpty(taiKhoan.MaGv))
                return RedirectToAction("Login", "Account", new { area = "" });

            var maGV = taiKhoan.MaGv;

            // Lấy danh sách lớp học phần của giảng viên này
            var lopHocPhans = await _context.LopHocPhans
                .Include(l => l.MaMhNavigation)
                .Include(l => l.MaGvNavigation)
                .Include(l => l.DangKyHocs)
                .Where(l => l.MaGv == maGV)
               
                .ToListAsync();

            return View(lopHocPhans);
        }

        // Hiển thị danh sách sinh viên kèm form điểm danh (radio)
        public async Task<IActionResult> DanhSachSinhVien(int maLHP)
        {
            var lopHocPhan = await _context.LopHocPhans
                .FirstOrDefaultAsync(l => l.MaLhp == maLHP);

            if (lopHocPhan == null) return NotFound();

            var sinhViens = await _context.DangKyHocs
                .Where(d => d.MaLhp == maLHP)
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

            return View(model); // ✅ gửi đúng DiemDanhViewModel
        }


        // POST: lưu kết quả điểm danh (chốt 1 lần cho tất cả sinh viên)
        [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> LuuDiemDanh(DiemDanhViewModel model)
            {
                if (model == null || model.SinhViens == null)
                {
                    TempData["Error"] = "Dữ liệu điểm danh không hợp lệ.";
                    return RedirectToAction("DanhSachSinhVien", new { maLHP = model?.MaLHP ?? 0 });
                }

                foreach (var sv in model.SinhViens)
                {
                    // Tạo đối tượng DiemDanh (entity)
                    var dd = new DiemDanh
                    {
                        MaLhp = model.MaLHP,
                        MaSv = sv.MaSV,
                        // Lưu ngày hiện tại (sử dụng DateTime nếu DB mapping là datetime)
                        // Nếu entity DiemDanh.NgayDiemDanh là DateOnly, chuyển đổi sau:
                        NgayDiemDanh = DateOnly.FromDateTime(DateTime.Now),
                        TrangThai = sv.TrangThai
                    };

                    _context.DiemDanhs.Add(dd);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Chốt điểm danh thành công!";
                return RedirectToAction("DanhSachSinhVien", new { maLHP = model.MaLHP });
            }
        // GET: Chọn sinh viên để thêm vào lớp
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
                    HoTen = s.HoTen,
                    DaChon = false
                }).ToList()
            };

            return View(model);
        }

        // POST: Thêm các sinh viên đã chọn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemSinhVien(DanhSachChonSVViewModel model)
        {
            if (model == null || model.SinhViens == null)
                return RedirectToAction("DanhSachSinhVien", new { maLHP = model?.MaLHP ?? 0 });

            var dsChon = model.SinhViens.Where(s => s.DaChon).ToList();
            foreach (var sv in dsChon)
            {
                if (!_context.DangKyHocs.Any(d => d.MaLhp == model.MaLHP && d.MaSv == sv.MaSV))
                {
                    _context.DangKyHocs.Add(new DangKyHoc
                    {
                        MaLhp = model.MaLHP,
                        MaSv = sv.MaSV,
                        NgayDangKy = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã thêm sinh viên vào lớp.";
            return RedirectToAction("DanhSachSinhVien", new { maLHP = model.MaLHP });
        }
        // POST: Xóa 1 đăng ký (xóa sinh viên khỏi lớp)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaDangKy(int MaLHP, string MaSV)
        {
            if (string.IsNullOrEmpty(MaSV))
                return BadRequest();

            var dk = await _context.DangKyHocs
                .FirstOrDefaultAsync(d => d.MaLhp == MaLHP && d.MaSv == MaSV);

            if (dk == null)
            {
                TempData["Error"] = "Không tìm thấy đăng ký để xóa.";
                return RedirectToAction("DanhSachSinhVien", new { maLHP = MaLHP });
            }

            // 🔹 Xóa dữ liệu điểm danh trước
            var ddList = await _context.DiemDanhs
                .Where(dd => dd.MaLhp == MaLHP && dd.MaSv == MaSV)
                .ToListAsync();

            if (ddList.Any())
            {
                _context.DiemDanhs.RemoveRange(ddList);
            }

            // 🔹 Sau đó mới xóa đăng ký học
            _context.DangKyHocs.Remove(dk);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa sinh viên khỏi lớp.";
            return RedirectToAction("DanhSachSinhVien", new { maLHP = MaLHP });
        }
        // 🔹 Hiển thị form điểm danh
        public async Task<IActionResult> DiemDanh(int maLHP)
        {
            var lopHocPhan = await _context.LopHocPhans
                .Include(l => l.DangKyHocs)
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
        [HttpPost]
        public async Task<IActionResult> DiemDanh(DiemDanhViewModel model)
        {
            foreach (var item in model.SinhViens)
            {
                // Kiểm tra xem bản ghi DiemDanh đã tồn tại cho sinh viên + ngày hôm nay chưa
                var existing = await _context.DiemDanhs
                    .FirstOrDefaultAsync(d => d.MaLhp == model.MaLHP
                                           && d.MaSv == item.MaSV
                                           && d.NgayDiemDanh == DateOnly.FromDateTime(DateTime.Now));

                if (existing != null)
                {
                    // Cập nhật trạng thái nếu đã có
                    existing.TrangThai = item.TrangThai;
                }
                else
                {
                    // Tạo mới bản ghi điểm danh
                    var dd = new DiemDanh
                    {
                        MaLhp = model.MaLHP,
                        MaSv = item.MaSV,
                        NgayDiemDanh = DateOnly.FromDateTime(DateTime.Now),
                        TrangThai = item.TrangThai
                    };
                    _context.DiemDanhs.Add(dd);
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Đã lưu điểm danh!";
            return RedirectToAction(nameof(DiemDanh), new { maLHP = model.MaLHP });
        }



        // GET: Hiển thị form nhập điểm
        // ========================== NHẬP ĐIỂM ==========================
        public async Task<IActionResult> NhapDiem(int maLHP)
        {
            var lop = await _context.LopHocPhans
                .Include(l => l.DangKyHocs)
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
                    .FirstOrDefaultAsync(d => d.MaLhp == model.MaLHP && d.MaSv == sv.MaSV);

                if (dk == null) continue;

                decimal cc = (decimal)(sv.DiemChuyenCan ?? 0);
                decimal gk = (decimal)(sv.DiemGiuaKy ?? 0);
                decimal ck = (decimal)(sv.DiemCuoiKy ?? 0);

                // Tính điểm tổng kết (có thể tùy chỉnh tỉ lệ)
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

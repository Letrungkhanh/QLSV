using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using student_management.Models;

namespace student_management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SinhViensController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public SinhViensController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        // ✅ DANH SÁCH SINH VIÊN
        public async Task<IActionResult> Index()
        {
            var sinhViens = await _context.SinhViens
                .Include(s => s.MaKhoaNavigation)
                .Include(s => s.MaLopNavigation)
                .ToListAsync();
            return View(sinhViens);
        }

        // ✅ CHI TIẾT
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaKhoaNavigation)
                .Include(s => s.MaLopNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);

            if (sinhVien == null)
                return NotFound();

            return View(sinhVien);
        }

        // ✅ HIỂN THỊ FORM THÊM MỚI
        public IActionResult Create()
        {
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa");
            ViewData["MaLop"] = new SelectList(_context.LopChinhQuies, "MaLop", "TenLop");
            return View();
        }

        // ✅ THÊM MỚI SINH VIÊN + ẢNH
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(student_management.Models.SinhVien sinhVien, IFormFile? AnhFile)
        {
            if (ModelState.IsValid)
            {
                if (AnhFile != null && AnhFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "sinhvien");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(AnhFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await AnhFile.CopyToAsync(stream);
                    }

                    sinhVien.Anh = "/uploads/sinhvien/" + fileName;
                }

                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            ViewData["MaLop"] = new SelectList(_context.LopChinhQuies, "MaLop", "TenLop", sinhVien.MaLop);
            return View(sinhVien);
        }

        // ✅ HIỂN THỊ FORM SỬA
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null)
                return NotFound();

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            ViewData["MaLop"] = new SelectList(_context.LopChinhQuies, "MaLop", "TenLop", sinhVien.MaLop);
            return View(sinhVien);
        }

        // ✅ CẬP NHẬT SINH VIÊN + ẢNH
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, student_management.Models.SinhVien sinhVien, IFormFile? AnhFile)
        {
            if (id != sinhVien.MaSv)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu có ảnh mới
                    if (AnhFile != null && AnhFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "sinhvien");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(AnhFile.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await AnhFile.CopyToAsync(stream);
                        }

                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(sinhVien.Anh))
                        {
                            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", sinhVien.Anh.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);
                        }

                        sinhVien.Anh = "/uploads/sinhvien/" + fileName;
                    }

                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SinhViens.Any(e => e.MaSv == sinhVien.MaSv))
                        return NotFound();
                    throw;
                }
            }

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            ViewData["MaLop"] = new SelectList(_context.LopChinhQuies, "MaLop", "TenLop", sinhVien.MaLop);
            return View(sinhVien);
        }

        // ✅ XÓA
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaKhoaNavigation)
                .Include(s => s.MaLopNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);

            if (sinhVien == null)
                return NotFound();

            return View(sinhVien);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien != null)
            {
                // Xóa ảnh vật lý nếu có
                if (!string.IsNullOrEmpty(sinhVien.Anh))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", sinhVien.Anh.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                _context.SinhViens.Remove(sinhVien);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

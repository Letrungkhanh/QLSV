using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using student_management.Models;

namespace student_management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GiaoViensController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public GiaoViensController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        // GET: Admin/GiaoViens
        public async Task<IActionResult> Index()
        {
            var quanlyhocDbContext = _context.GiaoViens.Include(g => g.MaKhoaNavigation);
            return View(await quanlyhocDbContext.ToListAsync());
        }

        // GET: Admin/GiaoViens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giaoVien = await _context.GiaoViens
                .Include(g => g.MaKhoaNavigation)
                .FirstOrDefaultAsync(m => m.MaGv == id);
            if (giaoVien == null)
            {
                return NotFound();
            }

            return View(giaoVien);
        }

        // GET: Admin/GiaoViens/Create
        public IActionResult Create()
        {
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiaoVien giaoVien, IFormFile? AnhFile)
        {
            if (ModelState.IsValid)
            {
                if (AnhFile != null && AnhFile.Length > 0)
                {
                    // ✅ Lưu vào thư mục wwwroot/uploads
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(AnhFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await AnhFile.CopyToAsync(stream);
                    }

                    // ✅ Lưu đường dẫn tương đối vào DB
                    giaoVien.Anh = "/uploads/" + fileName;
                }

                _context.Add(giaoVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", giaoVien.MaKhoa);
            return View(giaoVien);
        }

        // GET: Admin/GiaoViens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var giaoVien = await _context.GiaoViens.FindAsync(id);
            if (giaoVien == null)
                return NotFound();

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", giaoVien.MaKhoa);
            return View(giaoVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, GiaoVien giaoVien, IFormFile? AnhFile)
        {
            if (id != giaoVien.MaGv)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var giaoVienCu = await _context.GiaoViens.AsNoTracking().FirstOrDefaultAsync(g => g.MaGv == id);
                    if (giaoVienCu == null)
                        return NotFound();

                    // Nếu có ảnh mới
                    if (AnhFile != null && AnhFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(AnhFile.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await AnhFile.CopyToAsync(stream);
                        }

                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(giaoVienCu.Anh))
                        {
                            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", giaoVienCu.Anh.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);
                        }

                        giaoVien.Anh = "/uploads/" + fileName;
                    }
                    else
                    {
                        // ✅ Nếu không chọn ảnh mới, giữ nguyên ảnh cũ
                        giaoVien.Anh = giaoVienCu.Anh;
                    }

                    _context.Update(giaoVien);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.GiaoViens.Any(e => e.MaGv == giaoVien.MaGv))
                        return NotFound();
                    throw;
                }
            }

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", giaoVien.MaKhoa);
            return View(giaoVien);
        }





        // GET: Admin/GiaoViens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giaoVien = await _context.GiaoViens
                .Include(g => g.MaKhoaNavigation)
                .FirstOrDefaultAsync(m => m.MaGv == id);
            if (giaoVien == null)
            {
                return NotFound();
            }

            return View(giaoVien);
        }

        // POST: Admin/GiaoViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var giaoVien = await _context.GiaoViens.FindAsync(id);
            if (giaoVien != null)
            {
                _context.GiaoViens.Remove(giaoVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiaoVienExists(string id)
        {
            return _context.GiaoViens.Any(e => e.MaGv == id);
        }
    }
}

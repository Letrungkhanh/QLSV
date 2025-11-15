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
    public class MonHocsController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public MonHocsController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        // GET: Admin/MonHocs
        public async Task<IActionResult> Index()
        {
            var quanlyhocDbContext = _context.MonHocs.Include(m => m.MaKhoaNavigation);
            return View(await quanlyhocDbContext.ToListAsync());
        }

        // GET: Admin/MonHocs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monHoc = await _context.MonHocs
                .Include(m => m.MaKhoaNavigation)
                .FirstOrDefaultAsync(m => m.MaMh == id);
            if (monHoc == null)
            {
                return NotFound();
            }

            return View(monHoc);
        }

        // GET: Admin/MonHocs/Create
        public IActionResult Create()
        {
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa"); // ✅ Hiển thị tên khoa
            return View();
        }

        // POST: Admin/MonHocs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaMh,TenMh,SoTinChi,Anh,MaKhoa")] MonHoc monHoc, IFormFile? uploadAnh)
        {
            // Kiểm tra trùng Mã môn học
            var existsCode = await _context.MonHocs
                .AnyAsync(m => m.MaMh.Trim().ToLower() == monHoc.MaMh.Trim().ToLower());
            if (existsCode)
            {
                ModelState.AddModelError("", $"Mã môn học '{monHoc.MaMh}' đã tồn tại.");
                ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", monHoc.MaKhoa);
                return View(monHoc);
            }

            // Kiểm tra trùng Tên môn học
            var existsName = await _context.MonHocs
                .AnyAsync(m => m.TenMh.Trim().ToLower() == monHoc.TenMh.Trim().ToLower());
            if (existsName)
            {
                ModelState.AddModelError("", $"Tên môn học '{monHoc.TenMh}' đã tồn tại.");
                ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", monHoc.MaKhoa);
                return View(monHoc);
            }

            if (ModelState.IsValid)
            {
                // Nếu có upload ảnh
                if (uploadAnh != null && uploadAnh.Length > 0)
                {
                    var fileName = Path.GetFileName(uploadAnh.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await uploadAnh.CopyToAsync(stream);
                    }

                    monHoc.Anh = "/images/" + fileName;
                }

                _context.Add(monHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", monHoc.MaKhoa);
            return View(monHoc);
        }


        // GET: Admin/MonHocs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monHoc = await _context.MonHocs.FindAsync(id);
            if (monHoc == null)
            {
                return NotFound();
            }

            // ✅ Hiển thị tên khoa trong dropdown
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", monHoc.MaKhoa);
            return View(monHoc);
        }

        // POST: Admin/MonHocs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaMh,TenMh,SoTinChi,Anh,MaKhoa")] MonHoc monHoc, IFormFile? uploadAnh)
        {
            if (id != monHoc.MaMh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMonHoc = await _context.MonHocs.AsNoTracking().FirstOrDefaultAsync(m => m.MaMh == id);
                    if (existingMonHoc == null)
                    {
                        return NotFound();
                    }

                    // ✅ Nếu có upload ảnh mới thì lưu lại
                    if (uploadAnh != null && uploadAnh.Length > 0)
                    {
                        var fileName = Path.GetFileName(uploadAnh.FileName);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                        using (var stream = new FileStream(uploadPath, FileMode.Create))
                        {
                            await uploadAnh.CopyToAsync(stream);
                        }
                        monHoc.Anh = "/images/" + fileName;
                    }
                    else
                    {
                        // ✅ Nếu không upload ảnh mới thì giữ nguyên ảnh cũ
                        monHoc.Anh = existingMonHoc.Anh;
                    }

                    _context.Update(monHoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonHocExists(monHoc.MaMh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", monHoc.MaKhoa);
            return View(monHoc);
        }

        // GET: Admin/MonHocs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monHoc = await _context.MonHocs
                .Include(m => m.MaKhoaNavigation)
                .FirstOrDefaultAsync(m => m.MaMh == id);
            if (monHoc == null)
            {
                return NotFound();
            }

            return View(monHoc);
        }

        // POST: Admin/MonHocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var monHoc = await _context.MonHocs.FindAsync(id);
            if (monHoc != null)
            {
                _context.MonHocs.Remove(monHoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonHocExists(string id)
        {
            return _context.MonHocs.Any(e => e.MaMh == id);
        }
    }
}

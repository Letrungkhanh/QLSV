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
    public class SinhViensController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public SinhViensController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SinhViens
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách sinh viên kèm tên lớp và tên khoa
            var sinhViens = await _context.SinhViens
                .Include(s => s.MaKhoaNavigation)  // liên kết với bảng Khoa
                .Include(s => s.MaLopNavigation)   // liên kết với bảng LopChinhQui
                .ToListAsync();

            return View(sinhViens);
        }

        // GET: Admin/SinhViens/Details/5
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

        // GET: Admin/SinhViens/Create
        public IActionResult Create()
        {
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa");
            ViewData["MaLop"] = new SelectList(_context.LopChinhQuies, "MaLop", "TenLop");
            return View();
        }

        // POST: Admin/SinhViens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSv,HoTen,NgaySinh,Email,SoDienThoai,MaLop,MaKhoa,NamNhapHoc")] SinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            ViewData["MaLop"] = new SelectList(_context.LopChinhQuies, "MaLop", "TenLop", sinhVien.MaLop);
            return View(sinhVien);
        }

        // GET: Admin/SinhViens/Edit/5
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

        // POST: Admin/SinhViens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSv,HoTen,NgaySinh,Email,SoDienThoai,MaLop,MaKhoa,NamNhapHoc")] SinhVien sinhVien)
        {
            if (id != sinhVien.MaSv)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinhVienExists(sinhVien.MaSv))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            ViewData["MaLop"] = new SelectList(_context.LopChinhQuies, "MaLop", "TenLop", sinhVien.MaLop);
            return View(sinhVien);
        }

        // GET: Admin/SinhViens/Delete/5
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

        // POST: Admin/SinhViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien != null)
                _context.SinhViens.Remove(sinhVien);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SinhVienExists(string id)
        {
            return _context.SinhViens.Any(e => e.MaSv == id);
        }
    }
}

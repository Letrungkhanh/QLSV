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
    public class KhoasController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public KhoasController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Khoas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Khoas.ToListAsync());
        }

        // GET: Admin/Khoas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin Khoa + danh sách Lớp chính quy thuộc Khoa
            var khoa = await _context.Khoas
                .Include(k => k.LopChinhQuies) // tên navigation property trong model Khoa
                .FirstOrDefaultAsync(k => k.MaKhoa == id);

            if (khoa == null)
            {
                return NotFound();
            }

            return View(khoa);
        }

        // GET: Admin/Khoas/Create
        public IActionResult Create()
        {
            ViewBag.MaVien = new SelectList(_context.Viens, "MaVien", "TenVien");
            return View();
        }


        // POST: Admin/Khoas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKhoa,TenKhoa,MaVien")] Khoa khoa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khoa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Truyền lại danh sách viện nếu có lỗi để form không bị mất dropdown
            ViewBag.MaVien = new SelectList(_context.Viens, "MaVien", "TenVien", khoa.MaVien);
            return View(khoa);
        }

        // GET: Admin/Khoas/Edit/5
        // GET: Admin/Khoas/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoa = await _context.Khoas.FindAsync(id);
            if (khoa == null)
            {
                return NotFound();
            }

            // Truyền danh sách Viện để hiển thị trong dropdown
            ViewBag.MaVien = new SelectList(_context.Viens, "MaVien", "TenVien", khoa.MaVien);
            return View(khoa);
        }


        // POST: Admin/Khoas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaKhoa,TenKhoa,MaVien")] Khoa khoa)
        {
            if (id != khoa.MaKhoa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khoa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Khoas.Any(e => e.MaKhoa == khoa.MaKhoa))
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

            // Nếu có lỗi -> nạp lại danh sách viện để dropdown không bị trống
            ViewBag.MaVien = new SelectList(_context.Viens, "MaVien", "TenVien", khoa.MaVien);
            return View(khoa);
        }

        // GET: Admin/Khoas/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var khoa = await _context.Khoas
                .FirstOrDefaultAsync(m => m.MaKhoa == id);
            if (khoa == null)
                return NotFound();

            return View(khoa);
        }

        // POST: Admin/Khoas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string MaKhoa)
        {
            if (string.IsNullOrEmpty(MaKhoa))
                return NotFound();

            // Tìm khoa
            var khoa = await _context.Khoas.AsNoTracking().FirstOrDefaultAsync(k => k.MaKhoa == MaKhoa);
            if (khoa == null)
            {
                // Nếu không tìm thấy, không cần xóa nữa
                TempData["ErrorMessage"] = "Khoa không tồn tại hoặc đã bị xóa!";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Khoas.Remove(khoa);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa khoa thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Xảy ra lỗi khi xóa khoa, vui lòng thử lại!";
            }

            return RedirectToAction(nameof(Index));
        }




        private bool KhoaExists(string id)
        {
            return _context.Khoas.Any(e => e.MaKhoa == id);
        }
    }
}

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
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "MaKhoa");
            return View();
        }

        // POST: Admin/GiaoViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGv,HoTen,NgaySinh,Email,SoDienThoai,MaKhoa")] GiaoVien giaoVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(giaoVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "MaKhoa", giaoVien.MaKhoa);
            return View(giaoVien);
        }

        // GET: Admin/GiaoViens/Edit/5
        // GET: Admin/GiaoViens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giaoVien = await _context.GiaoViens.FindAsync(id);
            if (giaoVien == null)
            {
                return NotFound();
            }

            // 🟢 Truyền danh sách Khoa (hiển thị Tên Khoa thay vì Mã)
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", giaoVien.MaKhoa);
            return View(giaoVien);
        }


        // POST: Admin/GiaoViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaGv,HoTen,NgaySinh,Email,SoDienThoai,MaKhoa")] GiaoVien giaoVien)
        {
            if (id != giaoVien.MaGv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giaoVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiaoVienExists(giaoVien.MaGv))
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

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
    public class LopChinhQuysController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public LopChinhQuysController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LopChinhQuys
        public async Task<IActionResult> Index()
        {
            var quanlyhocDbContext = _context.LopChinhQuies.Include(l => l.MaKhoaNavigation);
            return View(await quanlyhocDbContext.ToListAsync());
        }

        // GET: Admin/LopChinhQuys/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lop = await _context.LopChinhQuies
                .Include(l => l.MaKhoaNavigation)
                .Include(l => l.SinhViens) // Lấy luôn danh sách sinh viên thuộc lớp
                .FirstOrDefaultAsync(m => m.MaLop == id);

            if (lop == null)
            {
                return NotFound();
            }

            return View(lop);
        }



        // GET: Admin/LopChinhQuys/Create
        public IActionResult Create()
        {
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "MaKhoa");
            return View();
        }

        // POST: Admin/LopChinhQuys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLop,TenLop,MaKhoa")] LopChinhQuy lopChinhQuy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lopChinhQuy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "MaKhoa", lopChinhQuy.MaKhoa);
            return View(lopChinhQuy);
        }

        // GET: Admin/LopChinhQuys/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopChinhQuy = await _context.LopChinhQuies.FindAsync(id);
            if (lopChinhQuy == null)
            {
                return NotFound();
            }
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "MaKhoa", lopChinhQuy.MaKhoa);
            return View(lopChinhQuy);
        }

        // POST: Admin/LopChinhQuys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaLop,TenLop,MaKhoa")] LopChinhQuy lopChinhQuy)
        {
            if (id != lopChinhQuy.MaLop)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lopChinhQuy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LopChinhQuyExists(lopChinhQuy.MaLop))
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
            ViewData["MaKhoa"] = new SelectList(_context.Khoas, "MaKhoa", "MaKhoa", lopChinhQuy.MaKhoa);
            return View(lopChinhQuy);
        }

        // GET: Admin/LopChinhQuys/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopChinhQuy = await _context.LopChinhQuies
                .Include(l => l.MaKhoaNavigation)
                .FirstOrDefaultAsync(m => m.MaLop == id);
            if (lopChinhQuy == null)
            {
                return NotFound();
            }

            return View(lopChinhQuy);
        }

        // POST: Admin/LopChinhQuys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var lopChinhQuy = await _context.LopChinhQuies.FindAsync(id);
            if (lopChinhQuy != null)
            {
                _context.LopChinhQuies.Remove(lopChinhQuy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LopChinhQuyExists(string id)
        {
            return _context.LopChinhQuies.Any(e => e.MaLop == id);
        }
    }
}

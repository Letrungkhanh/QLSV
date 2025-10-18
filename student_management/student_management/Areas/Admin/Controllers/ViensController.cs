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
    public class ViensController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public ViensController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Viens
        public async Task<IActionResult> Index()
        {
            return View(await _context.Viens.ToListAsync());
        }
        // GET: Admin/Viens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy viện và kèm theo danh sách khoa của viện đó
            var vien = await _context.Viens
                .Include(v => v.Khoas)
                .FirstOrDefaultAsync(v => v.MaVien == id);

            if (vien == null)
            {
                return NotFound();
            }

            return View(vien);
        }


        // GET: Admin/Viens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Viens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaVien,TenVien,MoTa")] Vien vien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vien);
        }

        // GET: Admin/Viens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vien = await _context.Viens.FindAsync(id);
            if (vien == null)
            {
                return NotFound();
            }
            return View(vien);
        }

        // POST: Admin/Viens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaVien,TenVien,MoTa")] Vien vien)
        {
            if (id != vien.MaVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VienExists(vien.MaVien))
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
            return View(vien);
        }

        // GET: Admin/Viens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vien = await _context.Viens
                .FirstOrDefaultAsync(m => m.MaVien == id);
            if (vien == null)
            {
                return NotFound();
            }

            return View(vien);
        }

        // POST: Admin/Viens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var vien = await _context.Viens.FindAsync(id);
            if (vien != null)
            {
                _context.Viens.Remove(vien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VienExists(string id)
        {
            return _context.Viens.Any(e => e.MaVien == id);
        }
    }
}

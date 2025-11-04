using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using student_management.Models;

namespace student_management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LopHocPhansController : Controller
    {
        private readonly QuanlyhocDbContext _context;

        public LopHocPhansController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LopHocPhans
        public async Task<IActionResult> Index()
        {
            var quanlyhocDbContext = _context.LopHocPhans.Include(l => l.MaGvNavigation).Include(l => l.MaMhNavigation);
            return View(await quanlyhocDbContext.ToListAsync());
        }

        // GET: Admin/LopHocPhans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var lopHocPhan = await _context.LopHocPhans
    .Include(l => l.MaGvNavigation)
    .Include(l => l.MaMhNavigation)
    .Include(l => l.DangKyHocs)
        .ThenInclude(d => d.MaSvNavigation) // để lấy thông tin sinh viên
    .FirstOrDefaultAsync(m => m.MaLhp == id);
            if (lopHocPhan == null)
            {
                return NotFound();
            }

            return View(lopHocPhan);
        }
        // ✅ GET: Admin/LopHocPhans/Create
        public IActionResult Create()
        {
            ViewBag.MonHocList = new SelectList(_context.MonHocs.ToList(), "MaMh", "TenMh");
            ViewBag.GiaoVienList = new SelectList(_context.GiaoViens.ToList(), "MaGv", "HoTen");
            return View();
        }

        // ✅ POST: Admin/LopHocPhans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenLhp,MaMh,MaGv,HocKy,NamHoc,SiSoToiDa")] LopHocPhan lopHocPhan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lopHocPhan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // ⚠️ Load lại dropdown nếu form có lỗi
            ViewBag.MonHocList = new SelectList(_context.MonHocs.ToList(), "MaMh", "TenMh", lopHocPhan.MaMh);
            ViewBag.GiaoVienList = new SelectList(_context.GiaoViens.ToList(), "MaGv", "HoTen", lopHocPhan.MaGv);

            return View(lopHocPhan);
        }

        // GET: Admin/LopHocPhans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHocPhan = await _context.LopHocPhans.FindAsync(id);
            if (lopHocPhan == null)
            {
                return NotFound();
            }

            // ⚠️ SỬA LỖI 1: Sửa 2 dòng dưới đây để hiển thị Tên thay vì Mã
            ViewData["MaGv"] = new SelectList(_context.GiaoViens, "MaGv", "HoTen", lopHocPhan.MaGv);
            ViewData["MaMh"] = new SelectList(_context.MonHocs, "MaMh", "TenMh", lopHocPhan.MaMh);
            return View(lopHocPhan);
        }

        // POST: Admin/LopHocPhans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            // ⚠️ SỬA LỖI 2: Xóa "TrangThai" và "SiSoHienTai" khỏi [Bind]
            // để giữ lại giá trị cũ trong database khi cập nhật.
            [Bind("MaLhp,TenLhp,MaMh,MaGv,HocKy,NamHoc,SiSoToiDa")] LopHocPhan lopHocPhan)
        {
            if (id != lopHocPhan.MaLhp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy bản ghi gốc từ DB
                    var entityToUpdate = await _context.LopHocPhans.FindAsync(id);
                    if (entityToUpdate == null) return NotFound();

                    // Cập nhật các trường từ form
                    entityToUpdate.TenLhp = lopHocPhan.TenLhp;
                    entityToUpdate.MaMh = lopHocPhan.MaMh;
                    entityToUpdate.MaGv = lopHocPhan.MaGv;
                    entityToUpdate.HocKy = lopHocPhan.HocKy;
                    entityToUpdate.NamHoc = lopHocPhan.NamHoc;
                    entityToUpdate.SiSoToiDa = lopHocPhan.SiSoToiDa;

                    // Không cập nhật TrangThai và SiSoHienTai (vì đã xóa khỏi Bind)
                    // EF Core sẽ tự động giữ lại giá trị cũ của chúng.
                    _context.Update(entityToUpdate);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LopHocPhanExists(lopHocPhan.MaLhp))
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

            // ⚠️ SỬA LỖI 3: Sửa 2 dòng này khi ModelState không hợp lệ (để tải lại View)
            // Phải giống như hàm GET Edit
            ViewData["MaGv"] = new SelectList(_context.GiaoViens, "MaGv", "HoTen", lopHocPhan.MaGv);
            ViewData["MaMh"] = new SelectList(_context.MonHocs, "MaMh", "TenMh", lopHocPhan.MaMh);
            return View(lopHocPhan);
        }

        // GET: Admin/LopHocPhans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHocPhan = await _context.LopHocPhans
                .Include(l => l.MaGvNavigation)
                .Include(l => l.MaMhNavigation)
                .FirstOrDefaultAsync(m => m.MaLhp == id);
            if (lopHocPhan == null)
            {
                return NotFound();
            }

            return View(lopHocPhan);
        }

        // POST: Admin/LopHocPhans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lopHocPhan = await _context.LopHocPhans.FindAsync(id);
            if (lopHocPhan != null)
            {
                _context.LopHocPhans.Remove(lopHocPhan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LopHocPhanExists(int id)
        {
            return _context.LopHocPhans.Any(e => e.MaLhp == id);
        }
    }
}
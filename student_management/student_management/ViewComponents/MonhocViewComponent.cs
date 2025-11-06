using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;

namespace student_management.ViewComponents
{
    public class MonhocViewComponent : ViewComponent
    {
        private readonly QuanlyhocDbContext _context;

        public MonhocViewComponent(QuanlyhocDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await _context.LopHocPhans
                 .Include(l => l.MaGvNavigation)
                 .Include(l => l.MaMhNavigation)
                 .Include(l => l.DangKyHocs)
                 .Select(lhp => new
                 {
                     TenLhp = lhp.TenLhp,
                     TenGv = lhp.MaGvNavigation != null ? lhp.MaGvNavigation.HoTen : "Chưa phân công",
                     SoTinChi = lhp.MaMhNavigation != null ? lhp.MaMhNavigation.SoTinChi : 0,
                     SiSoHienTai = lhp.DangKyHocs.Count(),
                     Anh = lhp.MaMhNavigation != null ? lhp.MaMhNavigation.Anh : "~/assets/img/course-1.jpg"
                 })
                 .ToListAsync();


            return View(data);
        }

    }
}

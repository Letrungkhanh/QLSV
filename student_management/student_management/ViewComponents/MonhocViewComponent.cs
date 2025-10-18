using Microsoft.AspNetCore.Mvc;
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
            int total = _context.SinhViens.Where(s => s.MaKhoa == "CNTT").Count();
            ViewBag.Total = total;
            var items = _context.MonHocs.ToList();
            return await Task.FromResult<IViewComponentResult>(View(items));
        }
    }
}

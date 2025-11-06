using Microsoft.AspNetCore.Mvc;
using student_management.Models;

namespace student_management.ViewComponents
{
    public class GiangvienViewComponent : ViewComponent
    {
        private readonly QuanlyhocDbContext _context;

        public GiangvienViewComponent(QuanlyhocDbContext context)
        {
            _context = context;
        }


        public IViewComponentResult Invoke()
        {
            // Lấy tất cả giảng viên có ảnh (nếu muốn lọc)
            var gvs = _context.GiaoViens
                .Select(g => new
                {
                    g.MaGv,
                    g.HoTen,
                    g.Email,
                    TenKhoa = g.MaKhoaNavigation.TenKhoa,
                    g.Anh // lấy ảnh từ CSDL
                }).Take(4)
                .ToList(); // hoặc .Take(4) nếu muốn giới hạn

            return View("Default", gvs);
        }
    }
}

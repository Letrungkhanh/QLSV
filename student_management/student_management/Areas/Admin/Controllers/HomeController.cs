using Microsoft.AspNetCore.Mvc;
using student_management.Models;
using student_management.Models.ViewModels;

namespace student_management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
       

        private readonly QuanlyhocDbContext _context;
        public HomeController(QuanlyhocDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var model = new AdminDashboardViewModel
            {
                TongVien = _context.Viens.Count(),
                TongKhoa = _context.Khoas.Count(),
                TongMonHoc = _context.MonHocs.Count(),
                TongLopChinhQuy = _context.LopChinhQuies.Count(),
                TongLopHocPhan = _context.LopHocPhans.Count(),
                TongGiangVien = _context.GiaoViens.Count(),
                TongSinhVien = _context.SinhViens.Count(),
                TongTaiKhoan = _context.TaiKhoans.Count(),
                TongThongBao = _context.ThongBaos.Count(),
            };
            return View(model);
        }
    }
}

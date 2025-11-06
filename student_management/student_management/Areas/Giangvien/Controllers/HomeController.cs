using Microsoft.AspNetCore.Mvc;
using student_management.Models;

namespace student_management.Areas.Giangvien.Controllers
{
    [Area("Giangvien")]
    public class HomeController : Controller
    {
       

        private readonly QuanlyhocDbContext _context;

        public HomeController(QuanlyhocDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.SoGV = _context.GiaoViens.Count();
            ViewBag.SoSV = _context.SinhViens.Count();
            ViewBag.SoKhoa = _context.Khoas.Count();
            ViewBag.SoLopcq = _context.LopChinhQuies.Count();
            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_management.Models;
using System.Diagnostics;

namespace student_management.Controllers
{
   // [Authorize] // ?? yêu c?u ph?i ??ng nh?p m?i vào ???c b?t k? action nào trong controller
    public class HomeController : Controller
        
    {

        private readonly QuanlyhocDbContext _context;
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(QuanlyhocDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // N?u ch?a ??ng nh?p => chuy?n ??n trang ??ng nh?p
                return RedirectToAction("Login", "Account");
            }
            ViewBag.SoGV = _context.GiaoViens.Count();
            ViewBag.SoSV = _context.SinhViens.Count();
            ViewBag.SoKhoa = _context.Khoas.Count();
            ViewBag.SoLopcq = _context.LopChinhQuies.Count();
            return View();
           
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

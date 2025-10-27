using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using student_management.Models;
using System.Diagnostics;

namespace student_management.Controllers
{
   // [Authorize] // ?? yêu c?u ph?i ??ng nh?p m?i vào ???c b?t k? action nào trong controller
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // N?u ch?a ??ng nh?p => chuy?n ??n trang ??ng nh?p
                return RedirectToAction("Login", "Account");
            }

            return View();
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

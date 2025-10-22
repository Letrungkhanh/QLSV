using Microsoft.AspNetCore.Mvc;

namespace student_management.Areas.Giangvien.Controllers
{
    public class HomeController : Controller
    {
        [Area("Giangvien")]   
        
        public IActionResult Index()
        {
            return View();
        }
    }
}

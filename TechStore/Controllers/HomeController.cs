using Microsoft.AspNetCore.Mvc;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contactenos()
        {
            return View();
        }

        public IActionResult AcercaDe()
        {
            return View();
        }
    }
}
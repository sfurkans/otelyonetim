using Microsoft.AspNetCore.Mvc;

namespace Otel_Yonetim.Controllers
{
    public class YonetimController : Controller
    {
        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("Rol")?.ToLower();
            if (rol != "admin")
                return RedirectToAction("Yetkisiz", "Hata");

            return View();
        }
    }
}

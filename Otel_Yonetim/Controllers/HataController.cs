using Microsoft.AspNetCore.Mvc;

namespace Otel_Yonetim.Controllers
{
    public class HataController : Controller
    {
        public IActionResult Yetkisiz()
        {
            return View(); // Views/Hata/Yetkisiz.cshtml
        }
    }
}

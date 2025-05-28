using Microsoft.AspNetCore.Mvc;
using Otel_Yonetim.Models;
using System.Linq;

namespace Otel_Yonetim.Controllers
{
    public class AnaPanelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnaPanelController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciAdi")))
                return RedirectToAction("Index", "Giris");

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("KullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("Rol");

            var duyurular = _context.Duyurular
                .OrderByDescending(d => d.Tarih)
                .ToList(); // Tüm duyurular

            return View(duyurular); // model olarak gönder
        }



    }
}

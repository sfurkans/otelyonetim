using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Otel_Yonetim.Models;

namespace Otel_Yonetim.Controllers
{
    public class IstatistikController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IstatistikController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool RolYetkili()
        {
            var rol = HttpContext.Session.GetString("Rol")?.ToLower();
            return rol == "admin" || rol == "muhasebe" ;
        }

        public IActionResult Index()
        {
            if (!RolYetkili())
                return RedirectToAction("Yetkisiz", "Hata");

            var bugun = DateTime.Today;
            var sonAy = bugun.AddMonths(-1);

            // Genel İstatistikler
            ViewBag.ToplamMusteri = _context.Musteriler.Count(m => m.AktifMi);
            ViewBag.BosOda = _context.Odalar.Count(o => o.Durum == "Boş");
            ViewBag.DoluOda = _context.Odalar.Count(o => o.Durum == "Dolu");
            ViewBag.ToplamRezervasyon = _context.Rezervasyonlar.Count();
            ViewBag.ToplamGelir = _context.Odemeler.Sum(o => (decimal?)o.Tutar) ?? 0;
            ViewBag.BugunkuRezervasyon = _context.Rezervasyonlar.Count(r => r.GirisTarihi == bugun);
            ViewBag.AylikRezervasyon = _context.Rezervasyonlar.Count(r => r.GirisTarihi >= sonAy);
            ViewBag.IptalRezervasyon = _context.Rezervasyonlar.Count(r => r.Durum == "İptal Edildi");

            return View();
        }
    }
}

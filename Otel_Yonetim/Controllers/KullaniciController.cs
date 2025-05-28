using Microsoft.AspNetCore.Mvc;
using Otel_Yonetim.Models;
using System.Linq;

namespace Otel_Yonetim.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KullaniciController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool RolYetkili()
        {
            var rol = HttpContext.Session.GetString("Rol")?.ToLower();
            return rol == "admin";
        }

        public IActionResult Index()
        {
            if (!RolYetkili())
                return RedirectToAction("Yetkisiz", "Hata");

            var liste = _context.Kullanicilar.ToList();
            return View(liste);
        }

        public IActionResult Ekle()
        {
            if (!RolYetkili())
                return RedirectToAction("Yetkisiz", "Hata");

            return View();
        }

        [HttpPost]
        public IActionResult Ekle(Kullanici k)
        {
            if (!RolYetkili())
                return RedirectToAction("Yetkisiz", "Hata");

            // Aynı kullanıcı adı var mı?
            bool kullaniciAdiVar = _context.Kullanicilar
                .Any(u => u.KullaniciAdi.Trim().ToLower() == k.KullaniciAdi.Trim().ToLower());

            if (kullaniciAdiVar)
            {
                ModelState.AddModelError("KullaniciAdi", "Bu kullanıcı adı zaten kullanılıyor.");
                return View(k);
            }

            _context.Kullanicilar.Add(k);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }



        public IActionResult Guncelle(int id)
        {
            if (!RolYetkili())
                return RedirectToAction("Yetkisiz", "Hata");

            var k = _context.Kullanicilar.Find(id);
            return View(k);
        }

        [HttpPost]
        public IActionResult Guncelle(Kullanici guncel)
        {
            if (!RolYetkili())
                return RedirectToAction("Yetkisiz", "Hata");

            bool ayniKullaniciAdiVar = _context.Kullanicilar
                .Any(u => u.KullaniciAdi.Trim().ToLower() == guncel.KullaniciAdi.Trim().ToLower()
                       && u.Id != guncel.Id);

            if (ayniKullaniciAdiVar)
            {
                ModelState.AddModelError("KullaniciAdi", "Bu kullanıcı adı başka bir kullanıcı tarafından kullanılıyor.");
                return View(guncel);
            }

            _context.Kullanicilar.Update(guncel);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }



        public IActionResult Sil(int id)
        {
            if (!RolYetkili())
                return RedirectToAction("Yetkisiz", "Hata");

            var k = _context.Kullanicilar.Find(id);
            _context.Kullanicilar.Remove(k);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

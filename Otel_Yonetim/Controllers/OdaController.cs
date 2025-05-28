using Microsoft.AspNetCore.Mvc;
using Otel_Yonetim.Models;

namespace Otel_Yonetim.Controllers
{
    public class OdaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OdaController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool OturumKontrol()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciAdi"));
        }

        private bool RolKontrol(params string[] izinliRoller)
        {
            var rol = HttpContext.Session.GetString("Rol");

            return rol != null && izinliRoller.Select(r => r.ToLower()).Contains(rol.ToLower());
        }


        public IActionResult Index()
        {
            if (!OturumKontrol() || !RolKontrol("admin", "resepsiyon"))
                return RedirectToAction("Yetkisiz", "Hata");

            var odalar = _context.Odalar.ToList();
            return View(odalar);
        }


        public IActionResult Ekle()
        {
            if (!OturumKontrol())
                return RedirectToAction("Index", "Giris");

            if (!RolKontrol("admin"))
                return RedirectToAction("Yetkisiz", "Hata");

            return View();
        }


        [HttpPost]
        public IActionResult Ekle(Oda o)
        {
            if (!OturumKontrol())
                return RedirectToAction("Index", "Giris");

            if (!RolKontrol("admin"))
                return RedirectToAction("Yetkisiz", "Hata");

            // Aynı oda numarası var mı kontrol et
            bool odaVarMi = _context.Odalar.Any(x => x.OdaNo == o.OdaNo);
            if (odaVarMi)
            {
                ModelState.AddModelError("OdaNo", "Bu oda numarası zaten kayıtlı.");
                return View(o); // aynı formu tekrar göster
            }

            _context.Odalar.Add(o);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }



        public IActionResult Guncelle(int id)
        {
            if (!OturumKontrol())
                return RedirectToAction("Index", "Giris");

            var rol = HttpContext.Session.GetString("Rol");
            var oda = _context.Odalar.Find(id);

            if (oda == null)
                return NotFound();

            if (rol != null && rol.ToLower() == "admin")
            {
                return View("Guncelle", oda); // admin için tam güncelleme formu
            }

            return RedirectToAction("Yetkisiz", "Hata");
        }




        [HttpPost]
        public IActionResult Guncelle(Oda o)
        {
            if (!OturumKontrol() || !RolKontrol("admin"))
                return RedirectToAction("Index", "Giris");

            // Aynı OdaNo başka bir odada var mı kontrol et
            bool ayniOdaNoVar = _context.Odalar.Any(x => x.OdaNo == o.OdaNo && x.Id != o.Id);
            if (ayniOdaNoVar)
            {
                ModelState.AddModelError("OdaNo", "Bu oda numarası başka bir odayla çakışıyor.");
                return View(o); // aynı formu göster
            }

            _context.Odalar.Update(o);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Sil(int id)
        {
            if (!OturumKontrol() || !RolKontrol("admin"))
                return RedirectToAction("Yetkisiz", "Hata");

            var oda = _context.Odalar.Find(id);
            if (oda != null)
            {
                _context.Odalar.Remove(oda);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}

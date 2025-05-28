using Microsoft.AspNetCore.Mvc;
using Otel_Yonetim.Models;
using System.Linq;

namespace Otel_Yonetim.Controllers
{
    public class MusteriController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MusteriController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool OturumKontrol()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciAdi"));
        }

        private bool YetkiVar(params string[] izinliRoller)
        {
            var rol = HttpContext.Session.GetString("Rol")?.ToLower();
            return izinliRoller.Select(r => r.ToLower()).Contains(rol);
        }

        // ✅ Yalnızca aktif müşterileri göster
        public IActionResult Index()
        {
            if (!OturumKontrol() || !YetkiVar("admin", "resepsiyon"))
                return RedirectToAction("Yetkisiz", "Hata");

            var musteriler = _context.Musteriler
                .Where(m => m.AktifMi)
                .ToList();

            return View(musteriler);
        }

        public IActionResult Ekle()
        {
            if (!OturumKontrol() || !YetkiVar("admin", "resepsiyon"))
                return RedirectToAction("Yetkisiz", "Hata");

            return View();
        }

        [HttpPost]
        public IActionResult Ekle(Musteri m)
        {
            if (!OturumKontrol() || !YetkiVar("admin", "resepsiyon"))
                return RedirectToAction("Yetkisiz", "Hata");

            // SADECE aktif müşterilerde çakışma kontrolü yapılır
            bool musteriVar = _context.Musteriler.Any(x =>
                x.AktifMi && (
                    x.AdSoyad.Trim().ToLower() == m.AdSoyad.Trim().ToLower() ||
                    x.Telefon.Trim() == m.Telefon.Trim() ||
                    x.Email.Trim().ToLower() == m.Email.Trim().ToLower()
                ));

            if (musteriVar)
            {
                ModelState.AddModelError(string.Empty, "Bu ad, telefon veya e-posta zaten kayıtlı bir müşteride var.");
                return View(m);
            }

            m.AktifMi = true;
            _context.Musteriler.Add(m);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }




        public IActionResult Guncelle(int id)
        {
            if (!OturumKontrol() || !YetkiVar("admin", "resepsiyon"))
                return RedirectToAction("Yetkisiz", "Hata");

            var musteri = _context.Musteriler.Find(id);
            return View(musteri);
        }

        [HttpPost]
        public IActionResult Guncelle(Musteri m)
        {
            if (!OturumKontrol() || !YetkiVar("admin", "resepsiyon"))
                return RedirectToAction("Yetkisiz", "Hata");

            // SADECE aktif müşterilerde çakışma kontrolü yapılır
            bool musteriVar = _context.Musteriler.Any(x =>
                x.Id != m.Id && x.AktifMi && (
                    x.AdSoyad.Trim().ToLower() == m.AdSoyad.Trim().ToLower() ||
                    x.Telefon.Trim() == m.Telefon.Trim() ||
                    x.Email.Trim().ToLower() == m.Email.Trim().ToLower()
                ));

            if (musteriVar)
            {
                ModelState.AddModelError(string.Empty, "Bu ad, telefon veya e-posta başka bir aktif müşteride kayıtlı.");
                return View(m);
            }

            _context.Musteriler.Update(m);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }




        // ✅ Soft delete: AktifMi = false
        public IActionResult Sil(int id)
        {
            if (!OturumKontrol() || !YetkiVar("admin", "resepsiyon"))
                return RedirectToAction("Yetkisiz", "Hata");

            var musteri = _context.Musteriler.Find(id);
            if (musteri != null)
            {
                musteri.AktifMi = false;
                _context.Musteriler.Update(musteri);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // ✅ Pasif müşterileri göster
        public IActionResult Arsiv()
        {
            if (!OturumKontrol() || !YetkiVar("admin"))
                return RedirectToAction("Yetkisiz", "Hata");

            var pasifMusteriler = _context.Musteriler
                .Where(m => !m.AktifMi)
                .ToList();

            return View(pasifMusteriler);
        }

        // ✅ Pasif müşteriyi geri getir
        public IActionResult GeriAl(int id)
        {
            var musteri = _context.Musteriler.Find(id);
            if (musteri != null)
            {
                musteri.AktifMi = true;
                _context.Musteriler.Update(musteri);
                _context.SaveChanges();
            }

            return RedirectToAction("Arsiv");
        }
    }
}

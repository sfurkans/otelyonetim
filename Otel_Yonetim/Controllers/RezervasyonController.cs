using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Otel_Yonetim.Models;

namespace Otel_Yonetim.Controllers
{
    public class RezervasyonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RezervasyonController(ApplicationDbContext context)
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

            var rezervasyonlar = _context.Rezervasyonlar
                .Where(r => r.Musteri != null && r.Oda != null && r.Durum != null)
                .Include(r => r.Musteri)
                .Include(r => r.Oda)
                .ToList();

            return View(rezervasyonlar);
        }

        public IActionResult Ekle()
        {
            if (!OturumKontrol() || !RolKontrol("admin", "resepsiyon"))
                return RedirectToAction("Yetkisiz", "Hata");

            ViewBag.Musteriler = _context.Musteriler
                .Where(m => m.AktifMi)
                .OrderBy(m => m.AdSoyad)
                .ToList();

            ViewBag.Odalar = _context.Odalar
                .Where(o => o.Durum == "Boş")
                .OrderBy(o => o.OdaNo)
                .ToList();

            // Tüm dolu rezervasyonların tarih aralıklarını gönder
            ViewBag.DoluTarihler = _context.Rezervasyonlar
    .Where(r => r.Durum == "Aktif")
    .AsEnumerable() // EF'den belleğe al
    .SelectMany(r =>
        Enumerable.Range(0, (r.CikisTarihi - r.GirisTarihi).Days)
            .Select(i => r.GirisTarihi.AddDays(i).ToString("yyyy-MM-dd"))
    ).ToList();


            return View();
        }



        [HttpPost]
        public IActionResult Ekle(Rezervasyon r)
        {
            if (!OturumKontrol() || !RolKontrol("admin", "resepsiyon"))
                return RedirectToAction("Index", "Giris");

            // ❗ Giriş tarihi, çıkış tarihinden sonra olamaz
            if (r.GirisTarihi >= r.CikisTarihi)
            {
                ModelState.AddModelError("", "Giriş tarihi, çıkış tarihinden önce olmalıdır.");
            }

            // Oda tarih çakışması kontrolü
            bool cakisma = _context.Rezervasyonlar.Any(x =>
                x.OdaId == r.OdaId &&
                ((r.GirisTarihi >= x.GirisTarihi && r.GirisTarihi < x.CikisTarihi) ||
                 (r.CikisTarihi > x.GirisTarihi && r.CikisTarihi <= x.CikisTarihi))
            );

            if (cakisma)
            {
                ModelState.AddModelError("", "Bu tarihlerde bu oda zaten rezerve edilmiştir.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Hata = "Form doğrulaması başarısız. Tüm alanları doğru girdiğinizden emin olun.";
                ViewBag.Musteriler = _context.Musteriler.ToList();
                ViewBag.Odalar = _context.Odalar.Where(o => o.Durum == "Boş").ToList();
                return View(r);
            }

            var oda = _context.Odalar.Find(r.OdaId);
            if (oda != null)
                oda.Durum = "Dolu";

            r.Durum = "Aktif";
            _context.Rezervasyonlar.Add(r);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }



        // ❌ Silmek yerine İPTAL ETME işlemi yapılır (soft delete)
        public IActionResult Sil(int id)
        {
            if (!OturumKontrol() || !RolKontrol("admin", "resepsiyon"))
                return RedirectToAction("Index", "Giris");

            var r = _context.Rezervasyonlar.Include(x => x.Oda).FirstOrDefault(x => x.Id == id);
            if (r != null)
            {
                r.Durum = "İptal Edildi";

                if (r.Oda != null)
                {
                    r.Oda.Durum = "Boş";
                    _context.Odalar.Update(r.Oda);
                }

                _context.Rezervasyonlar.Update(r); // Güncelle
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}

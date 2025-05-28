using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Otel_Yonetim.Models;
using SelectPdf;

namespace Otel_Yonetim.Controllers
{
    public class OdemeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OdemeController(ApplicationDbContext context)
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
            if (!OturumKontrol() || !RolKontrol("admin", "muhasebe", "resepsiyon"))
                return RedirectToAction("Yetkisiz", "Hata");

            var odemeler = _context.Odemeler.ToList();
            var rezervasyonlar = _context.Rezervasyonlar
                .Include(r => r.Musteri)
                .Include(r => r.Oda)
                .ToDictionary(r => r.Id);

            ViewBag.Rezervasyonlar = rezervasyonlar;
            return View(odemeler);
        }

        public IActionResult Ekle()
        {
            if (!OturumKontrol() || !RolKontrol("admin", "muhasebe", "resepsiyon"))
                return RedirectToAction("Index", "Giris");

            var rezervasyonlar = _context.Rezervasyonlar
                .Include(r => r.Musteri)
                .Include(r => r.Oda)
                .Where(r => r.Durum == "Aktif")
                .ToList();

            if (!rezervasyonlar.Any())
            {
                TempData["Mesaj"] = "Aktif rezervasyon bulunmamaktadır.";
                return RedirectToAction("Index");
            }

            var selectList = rezervasyonlar.Select(r => new
            {
                Id = r.Id,
                Ad = $"{r.Musteri.AdSoyad} - Oda {r.Oda.OdaNo} ({r.GirisTarihi.ToShortDateString()} - {r.CikisTarihi.ToShortDateString()})"
            });

            ViewBag.Rezervasyonlar = new SelectList(selectList, "Id", "Ad");
            return View(new Odeme { OdemeTarihi = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ekle([Bind("RezervasyonId,Tutar,OdemeTarihi,OdemeTuru")] Odeme odeme)
        {
            if (!OturumKontrol() || !RolKontrol("admin", "muhasebe", "resepsiyon"))
                return RedirectToAction("Index", "Giris");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(odeme);
                    _context.SaveChanges();
                    TempData["Mesaj"] = "Ödeme başarıyla kaydedildi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ödeme kaydedilirken bir hata oluştu: " + ex.Message);
                }
            }

            var rezervasyonlar = _context.Rezervasyonlar
                .Include(r => r.Musteri)
                .Include(r => r.Oda)
                .Where(r => r.Durum == "Aktif")
                .ToList();

            var selectList = rezervasyonlar.Select(r => new
            {
                Id = r.Id,
                Ad = $"{r.Musteri.AdSoyad} - Oda {r.Oda.OdaNo} ({r.GirisTarihi.ToShortDateString()} - {r.CikisTarihi.ToShortDateString()})"
            });

            ViewBag.Rezervasyonlar = new SelectList(selectList, "Id", "Ad", odeme.RezervasyonId);
            return View(odeme);
        }

        public IActionResult Sil(int id)
        {
            if (!OturumKontrol() || !RolKontrol("admin", "muhasebe", "resepsiyon"))
                return RedirectToAction("Index", "Giris");

            var odeme = _context.Odemeler.Find(id);
            if (odeme != null)
            {
                _context.Odemeler.Remove(odeme);
                _context.SaveChanges();
                TempData["Mesaj"] = "Ödeme başarıyla silindi.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Fatura(int id)
        {
            if (!OturumKontrol() || !RolKontrol("admin", "muhasebe" , "resepsiyon"))
                return RedirectToAction("Yetkisiz", "Hata");

            var odeme = _context.Odemeler.Find(id);
            if (odeme == null)
            {
                TempData["Hata"] = "Fatura bulunamadı.";
                return RedirectToAction("Index");
            }

            var rezervasyon = _context.Rezervasyonlar
                .Include(r => r.Musteri)
                .Include(r => r.Oda)
                .FirstOrDefault(r => r.Id == odeme.RezervasyonId);

            if (rezervasyon == null)
            {
                TempData["Hata"] = "Rezervasyon bilgileri bulunamadı.";
                return RedirectToAction("Index");
            }

            ViewBag.Rezervasyon = rezervasyon;
            return View(odeme);
        }

        public IActionResult FaturaPdf(int id)
        {
            if (!OturumKontrol() || !RolKontrol("admin", "muhasebe"))
                return RedirectToAction("Yetkisiz", "Hata");

            string url = Url.Action("Fatura", "Odeme", new { id }, Request.Scheme);
            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument doc = converter.ConvertUrl(url);
            byte[] pdf = doc.Save();
            doc.Close();

            return File(pdf, "application/pdf", $"Fatura_{id}.pdf");
        }
    }
}

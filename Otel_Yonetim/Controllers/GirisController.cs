using Microsoft.AspNetCore.Mvc;
using Otel_Yonetim.Models;
using System.Linq;

namespace Otel_Yonetim.Controllers
{
    public class GirisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GirisController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string kullaniciAdi, string sifre)
        {
            var kullanici = _context.Kullanicilar
                .FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi && k.Sifre == sifre);

            if (kullanici != null)
            {
                HttpContext.Session.SetString("KullaniciAdi", kullanici.KullaniciAdi);
                HttpContext.Session.SetString("Rol", kullanici.Rol);
                HttpContext.Session.SetInt32("KullaniciId", kullanici.Id);

                switch (kullanici.Rol.ToLower())
                {
                    case "admin":
                    case "resepsiyon":
                    case "muhasebe":
                        return RedirectToAction("Index", "AnaPanel");

                    default:
                        return RedirectToAction("Yetkisiz", "Hata");
                }

            }

            ViewBag.Hata = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }


        public IActionResult Cikis()
        {
            HttpContext.Session.Clear(); // tüm session'ı temizle
            return RedirectToAction("Index", "Giris");
        }


    }
}

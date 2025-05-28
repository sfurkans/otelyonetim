using Microsoft.AspNetCore.Mvc;
using Otel_Yonetim.Models;

public class DuyuruController : Controller
{
    private readonly ApplicationDbContext _context;

    public DuyuruController(ApplicationDbContext context)
    {
        _context = context;
    }
    private bool RolYetkili()
    {
        var rol = HttpContext.Session.GetString("Rol")?.ToLower();
        return rol == "admin"  ;
    }
    public IActionResult Index()
    {
        if (!RolYetkili())
            return RedirectToAction("Yetkisiz", "Hata");

        var liste = _context.Duyurular.OrderByDescending(d => d.Tarih).ToList();
        return View(liste);
    }

    public IActionResult Ekle() => View();

    [HttpPost]
    public IActionResult Ekle(Duyuru d)
    {
        if (!ModelState.IsValid)
            return View(d); // boş alan varsa formu yeniden göster

        bool ayniIcerikVar = _context.Duyurular
            .Any(x => x.Icerik.Trim().ToLower() == d.Icerik.Trim().ToLower());

        if (ayniIcerikVar)
        {
            ModelState.AddModelError("Icerik", "Aynı içerikte duyuru zaten mevcut.");
            return View(d);
        }

        d.Tarih = DateTime.Now;
        _context.Duyurular.Add(d);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }


    public IActionResult Guncelle(int id)
    {
        var duyuru = _context.Duyurular.Find(id);
        if (duyuru == null)
            return NotFound();

        return View(duyuru);
    }

    [HttpPost]
    public IActionResult Guncelle(Duyuru guncel)
    {
        if (!ModelState.IsValid)
            return View(guncel);

        bool ayniIcerikVar = _context.Duyurular
            .Any(d => d.Icerik.Trim().ToLower() == guncel.Icerik.Trim().ToLower() && d.Id != guncel.Id);

        if (ayniIcerikVar)
        {
            ModelState.AddModelError("Icerik", "Bu içerikte başka bir duyuru zaten var.");
            return View(guncel);
        }

        _context.Duyurular.Update(guncel);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }



    public IActionResult Sil(int id)
    {
        var duyuru = _context.Duyurular.Find(id);
        if (duyuru == null)
            return NotFound();

        _context.Duyurular.Remove(duyuru);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    


}

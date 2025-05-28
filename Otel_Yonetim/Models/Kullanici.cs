using System.ComponentModel.DataAnnotations;

namespace Otel_Yonetim.Models
{
    public class Kullanici
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
        [StringLength(50)]
        public string KullaniciAdi { get; set; }

        public string Sifre { get; set; }
        public string Rol { get; set; } // Admin, Personel vb.
    }
}

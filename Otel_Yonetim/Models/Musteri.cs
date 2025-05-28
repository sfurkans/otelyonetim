using System.ComponentModel.DataAnnotations;

namespace Otel_Yonetim.Models
{
    public class Musteri
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ad soyad boş bırakılamaz.")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Telefon numarası boş bırakılamaz.")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "E-posta boş bırakılamaz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }


        public bool AktifMi { get; set; } = true;

    }
}

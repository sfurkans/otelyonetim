using System.ComponentModel.DataAnnotations;

namespace Otel_Yonetim.Models
{
    public class Duyuru
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık boş bırakılamaz.")]
        public string Baslik { get; set; }

        [Required(ErrorMessage = "İçerik boş bırakılamaz.")]
        public string Icerik { get; set; }

        public DateTime Tarih { get; set; }
    }
}

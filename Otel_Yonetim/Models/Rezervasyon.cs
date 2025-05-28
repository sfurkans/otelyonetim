using System.ComponentModel.DataAnnotations;

namespace Otel_Yonetim.Models
{
    public class Rezervasyon
    {
        public int Id { get; set; }

        [Required]
        public int MusteriId { get; set; }

        [Required]
        public int OdaId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime GirisTarihi { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CikisTarihi { get; set; }

        public string? Durum { get; set; }

        public Musteri? Musteri { get; set; }
        public Oda? Oda { get; set; }

        

    }
}

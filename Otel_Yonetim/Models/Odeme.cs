using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Otel_Yonetim.Models
{
    public class Odeme
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Lütfen bir rezervasyon seçiniz")]
        public int RezervasyonId { get; set; }
        
        [Required(ErrorMessage = "Lütfen ödeme tutarını giriniz")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır")]
        [Display(Name = "Tutar")]
        public decimal Tutar { get; set; }
        
        [Required(ErrorMessage = "Lütfen ödeme tarihini seçiniz")]
        [DataType(DataType.Date)]
        [Display(Name = "Ödeme Tarihi")]
        public DateTime OdemeTarihi { get; set; }
        
        [Required(ErrorMessage = "Lütfen ödeme türünü seçiniz")]
        [StringLength(50, ErrorMessage = "Ödeme türü en fazla 50 karakter olabilir")]
        [Display(Name = "Ödeme Türü")]
        public string OdemeTuru { get; set; }
    }
}

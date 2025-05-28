namespace Otel_Yonetim.Models
{
    public class Oda
    {
        public int Id { get; set; }
        public string OdaNo { get; set; }
        public string Tip { get; set; }
        public string Durum { get; set; } // "Boş", "Dolu"
    }
}

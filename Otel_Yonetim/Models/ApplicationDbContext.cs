using Microsoft.EntityFrameworkCore;
using Otel_Yonetim.Models;

namespace Otel_Yonetim.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Oda> Odalar { get; set; }
        public DbSet<Rezervasyon> Rezervasyonlar { get; set; }
        public DbSet<Odeme> Odemeler { get; set; }
        public DbSet<Duyuru> Duyurular { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Oda>()
                .HasIndex(o => o.OdaNo)
                .IsUnique();

            modelBuilder.Entity<Rezervasyon>()
                .HasOne(r => r.Musteri)
                .WithMany()
                .HasForeignKey(r => r.MusteriId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rezervasyon>()
                .HasOne(r => r.Oda)
                .WithMany()
                .HasForeignKey(r => r.OdaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

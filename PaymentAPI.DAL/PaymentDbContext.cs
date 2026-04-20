using Microsoft.EntityFrameworkCore;
using PaymentAPI.DAL.Entities;

namespace PaymentAPI.DAL
{
    public class PaymentDbContext : DbContext
    {
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.OrderId).IsUnique();
                entity.HasIndex(e => e.ExternalPaymentTranscationId).IsUnique();
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.OrderId).IsRequired();
                entity.Property(e => e.ExternalPaymentTranscationId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.Currency).IsRequired().HasConversion<string>().HasMaxLength(10);
                entity.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(50);
                entity.Property(e => e.PaymentProvider).IsRequired().HasConversion<string>().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }

        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }
    }
}

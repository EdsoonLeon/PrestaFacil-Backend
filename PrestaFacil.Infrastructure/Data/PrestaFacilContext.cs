using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PrestaFacil.Domain.Entities;

namespace PrestaFacil.Infrastructure.Data
{
    public class PrestaFacilContext : DbContext
    {
        public PrestaFacilContext(DbContextOptions<PrestaFacilContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<Cuota> Cuotas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Configuracion> Configuraciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.ClienteId);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DNI).IsRequired().HasMaxLength(20);
            });

            // Prestamo
            modelBuilder.Entity<Prestamo>(entity =>
            {
                entity.HasKey(e => e.PrestamoId);
                entity.Property(e => e.Monto).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TasaInteres).HasColumnType("decimal(5,2)");
                entity.HasOne(e => e.Cliente)
                      .WithMany(c => c.Prestamos)
                      .HasForeignKey(e => e.ClienteId);
            });

            // Cuota
            modelBuilder.Entity<Cuota>(entity =>
            {
                entity.HasKey(e => e.CuotaId);
                entity.Property(e => e.MontoTotal).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Prestamo)
                      .WithMany(p => p.Cuotas)
                      .HasForeignKey(e => e.PrestamoId);
            });

            // Pago
            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasKey(e => e.PagoId);
                entity.Property(e => e.Monto).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Prestamo)
                      .WithMany(p => p.Pagos)
                      .HasForeignKey(e => e.PrestamoId);
                entity.HasOne(e => e.Cuota)
                      .WithMany(c => c.Pagos)
                      .HasForeignKey(e => e.CuotaId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
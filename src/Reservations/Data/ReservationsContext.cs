using Microsoft.EntityFrameworkCore;
using Reservations.Data.Entities;

namespace Reservations.Data
{
    public class ReservationsContext : DbContext
    {
        public ReservationsContext(DbContextOptions<ReservationsContext> options)
            : base(options)
        {
            
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>()
                .Property(b => b.Name)
                .IsRequired();
            modelBuilder.Entity<Tenant>()
                .Property(b => b.LastName)
                .IsRequired();
            modelBuilder.Entity<Tenant>()
                .Property(b => b.Email)
                .IsRequired();
            modelBuilder.Entity<Tenant>()
                .Property(b => b.PhoneNumber)
                .IsRequired();
        }
        
        public DbSet<Reservation> Reservations { get; set; }
        
        public DbSet<Tenant> Tenants { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;
using Rentals.Data.Entities;

namespace Rentals.Data
{
    public class RentalsContext : DbContext
    {
        public RentalsContext(DbContextOptions<RentalsContext> options)
            : base(options)
        {
            
        }

        public DbSet<Rental> Rentals { get; set; }
    }
}
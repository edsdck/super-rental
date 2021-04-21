using System;

namespace Reservations.Data.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        public int RentalId { get; set; }

        public DateTime StartDateUtc { get; set; }

        public DateTime EndDateUtc { get; set; }

        public int TenantId { get; set; }

        public Tenant Tenant { get; set; }
    }
}
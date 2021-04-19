using System;

namespace Rentals.Data.Entities
{
    public class Rental
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string OwnerId { get; set; }
    }
}
using System;

namespace WebApplication.Models
{
    public class ReservationViewModel
    {
        public int Id { get; set; }
        
        public int RentalId { get; set; }
        
        public DateTime? StartDateUtc { get; set; }

        public DateTime? EndDateUtc { get; set; }
        
        public string TenantName { get; set; }
        
        public string TenantLastName { get; set; }
        
        public string TenantPhoneNumber { get; set; }
        
        public string TenantEmail { get; set; }
    }
}
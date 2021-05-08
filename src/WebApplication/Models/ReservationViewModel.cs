using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class ReservationViewModel
    {
        public int Id { get; set; }
        
        public int RentalId { get; set; }
        
        [Required(ErrorMessage = "Įsiregistravimo datą privalomą nurodyti.")]
        public DateTime? StartDateUtc { get; set; }
        
        [Required(ErrorMessage = "Išsiregistravimo datą privalomą nurodyti.")]
        public DateTime? EndDateUtc { get; set; }
        
        [Required(ErrorMessage = "Nuomininko vardas privalomas.")]
        public string TenantName { get; set; }
        
        [Required(ErrorMessage = "Nuomininko pavardė privaloma.")]
        public string TenantLastName { get; set; }
        
        [Required(ErrorMessage = "Nuomininko tel. nr. privalomas.")]
        [Phone(ErrorMessage = "Telefono numerio formatas neteisingas")]
        public string TenantPhoneNumber { get; set; }
        
        [EmailAddress(ErrorMessage = "El. paštas nurodytas neteisingu formatu.")]
        public string TenantEmail { get; set; }
    }
}
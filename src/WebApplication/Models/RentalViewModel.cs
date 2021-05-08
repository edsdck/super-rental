using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class RentalViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pavadinimas yra privalomas")]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Adresas yra privalomas")]
        public string Address { get; set; }
    }
}
#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Rentals.Models.Dtos
{
    public class RentalDto
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }
        
        [Required]
        public string? Address { get; set; }
    }
}
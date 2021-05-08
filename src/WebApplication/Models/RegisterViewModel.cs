using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class RegisterViewModel
    {
        public string ReturnUrl { get; set; }

        [Required(ErrorMessage = "El. paštas yra privalomas")]
        [EmailAddress(ErrorMessage = "Neteisingas el. pašto formatas")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Slaptažodis nepateiktas")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Slaptažodis nepateiktas")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Slaptažodžiai nesutampa")]
        public string ConfirmPassword { get; set; }
    }
}
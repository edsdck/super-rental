using Microsoft.AspNetCore.Mvc;

namespace Rentals.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RentalsController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Rental-GET";
        }
        
        [HttpPost]
        public string Post(string text)
        {
            text += "-Rental";
            
            return text;
        }
    }
}
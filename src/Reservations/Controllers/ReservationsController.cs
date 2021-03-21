using Microsoft.AspNetCore.Mvc;

namespace Reservations.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationsController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Reservations-GET";
        }
        
        [HttpPost]
        public string Post(string text)
        {
            text += "-Reservations";
            
            return text;
        }
    }
}
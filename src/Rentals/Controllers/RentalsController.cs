using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rentals.Data;
using Rentals.Data.Entities;
using Rentals.Models.Dtos;

namespace Rentals.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class RentalsController : ControllerBase
    {
        private readonly RentalsContext _rentalsContext;

        public RentalsController(RentalsContext rentalsContext)
        {
            _rentalsContext = rentalsContext;
        }

        [HttpGet("{id:int:min(1)}")]
        [ActionName(nameof(GetByIdAsync))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(RentalDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<RentalDto>> GetByIdAsync(int id)
        {
            var rental = await _rentalsContext.Rentals
                .SingleOrDefaultAsync(rent => rent.Id == id);

            if (rental is null)
            {
                return NotFound();
            }

            var rentalDto = new RentalDto
            {
                Id = rental.Id,
                Name = rental.Name,
                Description = rental.Description,
                Address = rental.Address
            };
            
            return rentalDto;
        }
        
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> CreateAsync(RentalDto model)
        {
            var userId = User.FindFirstValue("sub");
            
            var rental = new Rental
            {
                Name = model.Name,
                Description = model.Description,
                Address = model.Address,
                OwnerId = userId
            };

            await _rentalsContext.Rentals.AddAsync(rental);
            await _rentalsContext.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetByIdAsync), new { id = rental.Id }, null);
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> UpdateAsync(RentalDto model)
        {
            if (model.Id <= 0)
            {
                return NotFound("Please include rental id.");
            }
            
            var rental = await _rentalsContext.Rentals
                .SingleOrDefaultAsync(rent => rent.Id == model.Id);

            if (rental is null)
            {
                return NotFound();
            }

            rental.Name = model.Name;
            rental.Description = model.Description;
            rental.Address = model.Address;

            _rentalsContext.Rentals.Update(rental);
            await _rentalsContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByIdAsync), new { id = rental.Id }, null);
        }
        
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var rental = await _rentalsContext.Rentals
                .SingleOrDefaultAsync(rent => rent.Id == id);

            if (rental is null)
            {
                return NotFound();
            }

            _rentalsContext.Rentals.Remove(rental);
            await _rentalsContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
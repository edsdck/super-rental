using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reservations.Data;
using Reservations.Data.Entities;
using Reservations.Models.Dtos;
using Reservations.Services;

namespace Reservations.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationsContext _reservationsContext;
        private readonly ICommunicationService _communicationService;

        public ReservationsController(ReservationsContext reservationsContext,
            ICommunicationService communicationService)
        {
            _reservationsContext = reservationsContext;
            _communicationService = communicationService;
        }

        [HttpGet("/rentals/{rentalId}/reservations")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(RentalDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<ReservationDto>>> GetAllByRentalIdAsync(int rentalId,
            DateTime? startDateUtc,
            DateTime? endDateUtc,
            [FromHeader]string authorization)
        {
            var exists = await _communicationService.GetIfRentalExists(rentalId, authorization);
            if (!exists)
            {
                return BadRequest("Rental with a specified id does not exist.");
            }
            
            var reservations = await _reservationsContext.Reservations
                .Include(res => res.Tenant)
                .AsNoTracking()
                .Where(res => res.RentalId == rentalId &&
                              (!startDateUtc.HasValue || startDateUtc.Value <= res.EndDateUtc) &&
                              (!endDateUtc.HasValue || endDateUtc.Value >= res.StartDateUtc))
                .OrderByDescending(res => res.StartDateUtc)
                .ToListAsync();
            
            if (!reservations?.Any() ?? true)
            {
                return NotFound();
            }

            return reservations.Select(res => new ReservationDto
            {
                Id = res.Id,
                RentalId = res.RentalId,
                StartDateUtc = res.StartDateUtc,
                EndDateUtc = res.EndDateUtc,
                TenantName = res.Tenant.Name,
                TenantLastName = res.Tenant.LastName,
                TenantEmail = res.Tenant.Email,
                TenantPhoneNumber = res.Tenant.PhoneNumber
            }).ToList();
        }
        
        [HttpGet("{id:int:min(1)}")]
        [ActionName(nameof(GetByIdAsync))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(RentalDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ReservationDto>> GetByIdAsync(int id, [FromHeader]string authorization)
        {
            var reservation = await _reservationsContext.Reservations
                .Include(res => res.Tenant)
                .AsNoTracking()
                .SingleOrDefaultAsync(res => res.Id == id);

            if (reservation is null)
            {
                return NotFound();
            }
            
            var exists = await _communicationService.GetIfRentalExists(reservation.RentalId, authorization);
            
            if (!exists)
            {
                return NotFound("Rental with a specified id does not exist.");
            }

            return new ReservationDto
            {
                Id = reservation.Id,
                RentalId = reservation.RentalId,
                StartDateUtc = reservation.StartDateUtc,
                EndDateUtc = reservation.EndDateUtc,
                TenantName = reservation.Tenant.Name,
                TenantLastName = reservation.Tenant.LastName,
                TenantEmail = reservation.Tenant.Email,
                TenantPhoneNumber = reservation.Tenant.PhoneNumber
            };
        }
        
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateAsync(ReservationDto model, [FromHeader]string authorization)
        {
            if (model.RentalId <= 0)
            {
                return BadRequest("Rental id is not specified or specified incorrectly.");
            }
            
            var exists = await _communicationService.GetIfRentalExists(model.RentalId, authorization);

            if (!exists)
            {
                return BadRequest("Rental with a specified id does not exist.");
            }
            
            var isOverlapping = _reservationsContext.Reservations.Any(res => res.RentalId == model.RentalId &&
                                                                 model.StartDateUtc < res.EndDateUtc &&
                                                                 model.EndDateUtc > res.StartDateUtc);

            if (isOverlapping)
            {
                return BadRequest("Date is already booked.");
            }
            
            var reservation = new Reservation
            {
                RentalId = model.RentalId,
                StartDateUtc = model.StartDateUtc,
                EndDateUtc = model.EndDateUtc,
                Tenant = new Tenant
                {
                    Name = model.TenantName,
                    LastName = model.TenantLastName,
                    Email = model.TenantEmail,
                    PhoneNumber = model.TenantPhoneNumber
                }
            };

            await _reservationsContext.Reservations.AddAsync(reservation);
            await _reservationsContext.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetByIdAsync), new { id = reservation.Id }, null);
        }
        
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> UpdateAsync(ReservationDto model, [FromHeader]string authorization)
        {
            if (model.RentalId <= 0 || model.Id <= 0)
            {
                return NotFound("Please include correct rental and reservation ids.");
            }
            
            var exists = await _communicationService.GetIfRentalExists(model.RentalId, authorization);

            if (!exists)
            {
                return NotFound("Rental with a specified id does not exist.");
            }
            
            var reservation = await _reservationsContext.Reservations
                .Include(res => res.Tenant)
                .SingleOrDefaultAsync(res => res.Id == model.Id);

            if (reservation is null)
            {
                return NotFound("Reservation with a specified id does not exist.");
            }
            
            var isOverlapping = _reservationsContext.Reservations.Any(res => res.RentalId == model.RentalId &&
                                                                             model.StartDateUtc < res.EndDateUtc &&
                                                                             model.EndDateUtc > res.StartDateUtc &&
                                                                             res.Id != model.Id);
            if (isOverlapping)
            {
                return BadRequest("Date is already booked.");
            }
            
            reservation.StartDateUtc = model.StartDateUtc;
            reservation.EndDateUtc = model.EndDateUtc;
            reservation.Tenant.Name = model.TenantName;
            reservation.Tenant.LastName = model.TenantLastName;
            reservation.Tenant.Email = model.TenantEmail;
            reservation.Tenant.PhoneNumber = model.TenantPhoneNumber;

            _reservationsContext.Reservations.Update(reservation);
            await _reservationsContext.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetByIdAsync), new { id = reservation.Id }, null);
        }
        
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteAsync(int id, [FromHeader]string authorization)
        {
            var reservation = await _reservationsContext.Reservations
                .SingleOrDefaultAsync(rent => rent.Id == id);

            if (reservation is null)
            {
                return NotFound();
            }
            
            var exists = await _communicationService.GetIfRentalExists(reservation.RentalId, authorization);
            if (!exists)
            {
                return NotFound("Rental with a specified id does not exist.");
            }

            _reservationsContext.Reservations.Remove(reservation);
            await _reservationsContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
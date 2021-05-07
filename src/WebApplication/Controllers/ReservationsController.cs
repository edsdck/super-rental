using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Extensions;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly IGatewayService _gatewayService;

        public ReservationsController(IGatewayService gatewayService)
        {
            _gatewayService = gatewayService;
        }

        [HttpGet]
        public async Task<IActionResult> Info(int id, int rentalId)
        {
            var reservation = await _gatewayService.Get<ReservationViewModel>($"reservations/{id}");

            return reservation.IsSuccess
                ? View(reservation.Content)
                : RedirectToAction("Details", "Rentals", new {id = rentalId}).WithWarning("Ooops://", reservation.ErrorMessage);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int rentalId)
        {
            var reservation = await _gatewayService.Get<ReservationViewModel>($"reservations/{id}");
            
            return reservation.IsSuccess
                ? View(reservation.Content)
                : RedirectToAction(nameof(Info), new { id, rentalId })
                    .WithWarning("Ooops://", reservation.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ReservationViewModel model)
        {
            var reservation = await _gatewayService.Put("reservations", model);

            return reservation.IsSuccess
                ? RedirectToAction(nameof(Info), new {id = model.Id, rentalId = model.RentalId})
                    .WithSuccess("Yes!", "Rezervacija atnaujinta sėkmingai!")
                : RedirectToAction(nameof(Edit), new {id = model.Id, rentalId = model.RentalId})
                    .WithWarning("Ooops:/", reservation.ErrorMessage);
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int id, int rentalId)
        {
            var response = await _gatewayService.Delete($"reservations/{id}");

            return response.IsSuccess ?
                RedirectToAction("Details", "Rentals", new { id = rentalId }).WithSuccess("Įvykdyta!", "Nuomos objektas ištrintas.") :
                RedirectToAction(nameof(Edit), new { id, rentalId }).WithWarning("Ooops:/", response.ErrorMessage);
        }

        [HttpGet]
        public IActionResult Create([FromQuery]int rentalId)
        {
            var model = new ReservationViewModel
            {
                RentalId = rentalId
            };
            
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(ReservationViewModel model)
        {
            var response = await _gatewayService.Post("reservations", model);

            var reservation = response.IsSuccess
                ? (await _gatewayService.Get<ReservationViewModel>(response.LocationHeaderAbsolutePath))?.Content
                : null;

            return response.IsSuccess
                ? RedirectToAction("Info", "Reservations", new {id = reservation?.Id})
                    .WithSuccess("Yay", "Rezervacija pridėta!")
                : RedirectToAction("Details", "Rentals", new {id = model.RentalId})
                    .WithWarning("Nepavyko:(", response.ErrorMessage);
        }
    }
}
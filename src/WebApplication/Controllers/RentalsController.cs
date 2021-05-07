using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Extensions;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [Authorize]
    public class RentalsController : Controller
    {
        private readonly IGatewayService _gatewayService;

        public RentalsController(IGatewayService gatewayService)
        {
            _gatewayService = gatewayService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rentals = await _gatewayService.Get<IList<RentalViewModel>>("rentals/me");

            return rentals.IsSuccess ?
                View(rentals.Content) : 
                View().WithWarning("Oops!", rentals.ErrorMessage);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id,
            ReservationSearchModel searchModel)
        {
            var rental = await _gatewayService.Get<RentalViewModel>($"rentals/{id}");

            var reservations = await _gatewayService
                .Get<IList<ReservationViewModel>>($"/rentals/{id}/reservations");

            // filtering should be done on backend, not here. oh no... anyway
            IEnumerable<ReservationViewModel> filteredReservations = reservations.Content;
            if (!string.IsNullOrEmpty(searchModel.FromDate) &&
                DateTime.TryParse(searchModel.FromDate, out var fromDate))
            {
                filteredReservations =
                    reservations.Content.Where(res => res.StartDateUtc >= fromDate);
            }
            if (!string.IsNullOrEmpty(searchModel.ToDate) &&
                DateTime.TryParse(searchModel.ToDate, out var toDate))
            {
                filteredReservations =
                    reservations.Content.Where(res => res.EndDateUtc <= toDate);
            }
            
            var viewModel = new RentalDetailsViewModel
            {
                Details = rental.Content,
                Reservations = filteredReservations?.ToList() ?? new List<ReservationViewModel>()
            };
            viewModel.ReservationSearchModel.FromDate = searchModel.FromDate;
            viewModel.ReservationSearchModel.ToDate = searchModel.ToDate;

            return rental.IsSuccess
                ? View(viewModel)
                : RedirectToAction(nameof(Index))
                    .WithWarning("Oops!", rental.ErrorMessage);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new RentalViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(RentalViewModel model)
        {
            var rental = await _gatewayService.Post("rentals", model);

            return rental.IsSuccess ?
                RedirectToAction(nameof(Index)).WithSuccess("Puiku!", "Sukurtas naujas nuomos objektas.") : 
                View().WithWarning("Oops!", rental.ErrorMessage);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var rental = await _gatewayService.Get<RentalViewModel>($"rentals/{id}");
            
            return rental.IsSuccess ?
                View(rental.Content) :
                View().WithWarning("Oops!", rental.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RentalViewModel model)
        {
            var rental = await _gatewayService.Put("rentals", model);

            return rental.IsSuccess ?
                RedirectToAction(nameof(Edit), new { id = model.Id }).WithSuccess("Yay!", "Sėkmingai atnaujinote nuomos objektą.") :
                RedirectToAction(nameof(Edit), new { id = model.Id }).WithWarning("Oops!", rental.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string confirmationInput)
        {
            var rental = await _gatewayService.Get<RentalViewModel>($"rentals/{id}");

            if (!rental.IsSuccess)
            {
                return RedirectToAction(nameof(Edit), new { id }).WithWarning("Oops!", rental.ErrorMessage);
            }
            
            if (confirmationInput != rental.Content.Name)
            {
                return RedirectToAction(nameof(Edit), new { id }).WithWarning("Oops!", 
                    "Neteisingai įvesta patvirtinimo žinutė.");
            }

            var response = await _gatewayService.Delete($"rentals/{id}");

            return response.IsSuccess ?
                RedirectToAction(nameof(Index)).WithSuccess("Yay!", $"Nuomos objektas {rental.Content.Name} ištrintas.") :
                RedirectToAction(nameof(Edit), new { id }).WithWarning("Ooops:/", rental.ErrorMessage);
        }
    }
}
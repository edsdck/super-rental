using System.Collections.Generic;

namespace WebApplication.Models
{
    public class RentalDetailsViewModel
    {
        public RentalDetailsViewModel()
        {
            ReservationSearchModel = new ReservationSearchModel();
        }
        
        public RentalViewModel Details { get; set; }
        
        public IList<ReservationViewModel> Reservations { get; set; }
        
        public ReservationSearchModel ReservationSearchModel { get; set; }
    }
    
    public class ReservationSearchModel
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }
    }
}
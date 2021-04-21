using System.Threading.Tasks;
using Reservations.Models.Dtos;

namespace Reservations.Services
{
    public interface ICommunicationService
    {
        Task<bool> GetIfRentalExists(int rentalId, string bearerToken);
    }
}
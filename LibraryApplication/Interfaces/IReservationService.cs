using LibraryApplication.DTOs.Reservation;
using LibraryApplication.DTOs.Reservevations;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.Interfaces
{
    public interface IReservationService
    {
        Task ReserveBookAsync(ReserveBookRequest request);
        Task<IEnumerable<ReservationResponse>> GetMyActiveReservationsAsync();
        Task<IEnumerable<ReservationResponse>> GetPendingReservationsAsync();
        Task ConfirmReservationToLoanAsync(Guid reservationId);
        Task CancelReservationAsync(Guid reservationId);
    }
}

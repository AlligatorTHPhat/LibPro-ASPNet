using LibraryDomain.Entities;

namespace LibraryDomain.Interfaces
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation?> GetByIdAsync(Guid id);
        Task<IEnumerable<Reservation>> GetPendingAndReadyAsync();

        Task<IEnumerable<Reservation>> GetActiveReservationsByReaderAsync(Guid readerId);

        Task<Reservation?> GetReadyReservationByBookItemAsync(Guid bookItemId);

        Task<Reservation?> GetNextPendingReservationAsync(Guid bookItemId);

        Task<IEnumerable<Reservation>> GetExpiredReservationsAsync();
        Task<LoanDetail?> GetDetailByBarcodeAsync(string barcode);

        Task AddAsync(Reservation reservation);
        void Update(Reservation reservation);
    }
}
using LibraryDomain.Entities;
using LibraryDomain.Interfaces;
using LibraryDomain.Enums;
using LibraryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly LibraryDbContext _context;

        public ReservationRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations
                .Include(r => r.Reader)
                .Include(r => r.BookItem)
                    .ThenInclude(bi => bi.Book)
                .OrderByDescending(r => r.ReservedDate)
                .ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(Guid id)
        {
            return await _context.Reservations
                .Include(r => r.Reader)
                .Include(r => r.BookItem)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Reservation>> GetPendingAndReadyAsync()
        {
            return await _context.Reservations
                .Include(r => r.Reader)
                .Include(r => r.BookItem)
                    .ThenInclude(bi => bi.Book)
                .Where(r => r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Ready)
                .OrderBy(r => r.ReservedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetActiveReservationsByReaderAsync(Guid readerId)
        {
            return await _context.Reservations
                .Where(r => r.ReaderId == readerId &&
                           (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Ready))
                .OrderBy(r => r.ReservedDate)
                .ToListAsync();
        }

        public async Task<Reservation?> GetReadyReservationByBookItemAsync(Guid bookItemId)
        {
            return await _context.Reservations
                .FirstOrDefaultAsync(r => r.BookItemId == bookItemId &&
                                         r.Status == ReservationStatus.Ready);
        }

        public async Task<Reservation?> GetNextPendingReservationAsync(Guid bookItemId)
        {
            return await _context.Reservations
                .Where(r => r.BookItemId == bookItemId && r.Status == ReservationStatus.Pending)
                .OrderBy(r => r.ReservedDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Reservation>> GetExpiredReservationsAsync()
        {
            return await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Ready &&
                            r.ExpiryDate < DateTime.Now)
                .ToListAsync();
        }

        public async Task<LoanDetail?> GetDetailByBarcodeAsync(string barcode)
        {
            return await _context.LoanDetails
                .Include(ld => ld.BookItem)
                .FirstOrDefaultAsync(ld => ld.BookItem.Barcode == barcode);
        }

        public async Task AddAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
        }

        public void Update(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
        }
    }
}
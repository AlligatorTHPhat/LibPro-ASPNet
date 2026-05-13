using AutoMapper;
using LibraryApplication.DTOs.Loans;
using LibraryApplication.DTOs.Reservation;
using LibraryApplication.DTOs.Reservevations;
using LibraryApplication.Interfaces;
using LibraryDomain.Entities;
using LibraryDomain.Enums;
using LibraryDomain.Exceptions;
using LibraryDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IReaderRepository _readerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditRepository _auditRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILoanRepository _loanRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IMapper _mapper;

        public ReservationService(
        IReservationRepository reservationRepository,
        IBookRepository bookRepository,
        IReaderRepository readerRepository,
        IUnitOfWork unitOfWork,
        IAuditRepository auditRepository,
        ICurrentUserService currentUserService,
        ILoanRepository loanRepository,
        IStaffRepository staffRepository,
        IMapper mapper
        )
        {
            _reservationRepository = reservationRepository;
            _bookRepository = bookRepository;
            _readerRepository = readerRepository;
            _unitOfWork = unitOfWork;
            _auditRepository = auditRepository;
            _currentUserService = currentUserService;
            _loanRepository = loanRepository;
            _staffRepository = staffRepository;
            _mapper = mapper;
        }
        
        public async Task ReserveBookAsync(ReserveBookRequest request)
        {
            var userId = _currentUserService.UserId;
            var readerId = request.ReaderId.Value;

            var reader = await _readerRepository.GetByAccountIdAsync(userId)
                ?? throw new EntityNotFoundException("Reader", readerId);
            
            if (reader.HasOverdueLoans())
                throw new DomainException("Cant reserve books with overdue loans.");

            var bookItem = await _bookRepository.GetAvailableItemAsync(request.BookId, request.Barcode);

            if (bookItem == null)
                throw new DomainException("No available copies of the book.");

            var reservation = new Reservation(reader, bookItem);

            await _reservationRepository.AddAsync(reservation);
            await LogAction("Create Reservation", $"Reader {reader.Id} reserved BookItem {bookItem.Barcode}");
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<IEnumerable<ReservationResponse>> GetMyActiveReservationsAsync()
        {
            var userId = _currentUserService.UserId;

            var reader = await _readerRepository.GetByAccountIdAsync(userId)
                ?? throw new EntityNotFoundException("Reader", userId);

            var reservations = await _reservationRepository.GetActiveReservationsByReaderAsync(reader.Id);

            // 4. Map sang DTO Response
            return _mapper.Map<IEnumerable<ReservationResponse>>(reservations);
        }

        public async Task<IEnumerable<ReservationResponse>> GetPendingReservationsAsync()
        {
            var reservations = await _reservationRepository.GetPendingAndReadyAsync();
            return _mapper.Map<IEnumerable<ReservationResponse>>(reservations);
        }

        public async Task ConfirmReservationToLoanAsync(Guid reservationId)
        {
            var res = await _reservationRepository.GetByIdAsync(reservationId)
                ?? throw new EntityNotFoundException("Reservation", reservationId);

            if (res.Status != ReservationStatus.Ready)
                throw new DomainException("This reservation is not ready for confirmation.");

            if (res.ExpiryDate < DateTime.Now)
                throw new DomainException("This reservation has expired.");

            var staff = await _staffRepository.GetByAccountIdAsync(_currentUserService.UserId)
                ?? throw new DomainException("Staff information not found.");

            var loan = new Loan(res.Reader, staff);

            res.BookItem.MarkAsLoaned();

            loan.AddPhysicalItem(res.BookItem, 14);

            res.Complete();

            await _loanRepository.AddAsync(loan);
            _reservationRepository.Update(res);

            await LogAction("Confirm Reservation", $"Converted reservation {reservationId} to loan {loan.Id}");

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CancelReservationAsync(Guid reservationId)
        {
            var res = await _reservationRepository.GetByIdAsync(reservationId)
                ?? throw new EntityNotFoundException("Reservation", reservationId);

            res.Cancel();

            await LogAction("Cancel Reservation", $"Reservation {reservationId} was canceled");

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task LogAction(string action, string details)
        {
            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                action,
                "Reservation",
                "ReservationSystem",
                string.Empty,
                details
            );
            await _auditRepository.AddAsync(auditLog);
        }
    }
}

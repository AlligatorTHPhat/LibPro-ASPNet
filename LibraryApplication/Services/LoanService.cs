using AutoMapper;
using LibraryApplication.DTOs;
using LibraryApplication.DTOs.Loans;
using LibraryApplication.Interfaces;
using LibraryDomain.Entities;
using LibraryDomain.Enums;
using LibraryDomain.Exceptions;
using LibraryDomain.Interfaces;
using LibraryDomain.ValueObjects;
using static LibraryApplication.DTOs.Loans.CreateLoanRequest;

namespace LibraryApplication.Services
{
    public class LoanService : ILoanService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanDetailRepository _loanDetailRepository;
        private readonly IReaderRepository _readerRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ISecureTokenService _secureTokenService;
        private readonly IReservationRepository _reservationRepository;
        private readonly IPaymentService _paymentService;
        private readonly IAuditRepository _auditRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public LoanService(
            ICurrentUserService currentUserService,
            ILoanRepository loanRepository,
            ILoanDetailRepository loanDetailRepository,
            IReaderRepository readerRepository,
            IStaffRepository staffRepository,
            IBookRepository bookRepository,
            ISecureTokenService secureTokenService,
            IReservationRepository reservationRepository,
            IPaymentService paymentService,
            IAuditRepository auditRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _currentUserService = currentUserService;
            _loanRepository = loanRepository;
            _loanDetailRepository = loanDetailRepository;
            _readerRepository = readerRepository;
            _staffRepository = staffRepository;
            _bookRepository = bookRepository;
            _secureTokenService = secureTokenService;
            _reservationRepository = reservationRepository;
            _paymentService = paymentService;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private decimal CalculateFine(LoanDetail detail)
        {
            if (!detail.IsOverdue()) return 0;

            var overdueDays = (DateTime.Now - detail.DueDate).Days;
            return overdueDays * 5000;
        }

        public async Task<IEnumerable<LoanDetailResponse>> GetAllActiveLoanDetailsAsync()
        {
            var activeDetails = await _loanDetailRepository.GetAllActiveLoanDetailsAsync();

            return _mapper.Map<IEnumerable<LoanDetailResponse>>(activeDetails);
        }

        public async Task<LoanResponse> CreateOnlineLoanAsync(OnlineLoanRequest request, Guid readerAccountId, Guid staffAccountId)
        {
            //var reader = await _readerRepository.GetByIdAsync(readerId);
            var reader = await _readerRepository.GetByAccountIdAsync(readerAccountId);
            var staff = await _staffRepository.GetByAccountIdAsync(staffAccountId);

            if (reader == null) throw new Exception($"Cant find Reader with ID: {readerAccountId}");
            if (staff == null) throw new Exception($"Cant found system Librarian yet");
            var loan = new Loan(reader, staff);

            int currentActiveLoans = reader.Loans.SelectMany(l => l.Details)
                .Count(d => d.Status == LoanStatus.Active || d.Status == LoanStatus.Overdue);

            if (currentActiveLoans + request.BookItemsId.Count > reader.GetLoanLimit())
                throw new DomainException($"Your Limit is {reader.GetLoanLimit()}. You have a {currentActiveLoans} borrow book now.");

            foreach (var item in request.BookItemsId)
            {
                if (item.IsDigital)
                {
                    var bookItem = await _bookRepository.GetAvailableItemAsync(item.BookId, item.Barcode);
                    if (bookItem == null)
                        throw new Exception($"Book with Barcode {item.Barcode} is not avaiable yet!");

                        loan.AddDigitalBook(bookItem.Id, Guid.NewGuid().ToString(), request.LoanDays);
                }
                else
                {
                    var bookItem = await _bookRepository.GetAvailableItemAsync(item.BookId, item.Barcode);
                    if (bookItem == null)
                        throw new Exception($"Book with Barcode {item.Barcode} is not avaiable yet!");

                    if (reader.IsAlreadyBorrowingBook(bookItem.BookId))
                        throw new DomainException($"You areally borrow this book.");
                    bookItem.MarkAsLoaned();
                    loan.AddPhysicalItem(bookItem, request.LoanDays);
                }
            }

            var auditLog = new AuditLog(
                readerAccountId,
                "System/Reader",
                "Create Online Loan",
                "Loans",
                loan.Id.ToString(),
                string.Empty,
                $"Reader {readerAccountId} created an online loan for {request.BookItemsId.Count} items at {DateTime.Now}");

            await _auditRepository.AddAsync(auditLog);


            await _loanRepository.AddAsync(loan);
            await _unitOfWork.SaveChangesAsync();

            var loanWithData = await _loanRepository.GetByIdAsync(loan.Id);
            return _mapper.Map<LoanResponse>(loanWithData);
        }

        public async Task<LoanResponse> CreateDirectLoanAsync(DirectLoanRequest request, Guid staffAccountId)
        {
            var reader = await _readerRepository.GetByIdAsync(request.ReaderId);
            if (reader == null) throw new Exception($"Độc giả với ID {request.ReaderId} không tồn tại.");

            var staff = await _staffRepository.GetByAccountIdAsync(staffAccountId);
            if (staff == null) throw new Exception("Không xác định được nhân viên thực hiện giao dịch.");

            var loan = new Loan(reader, staff);

            foreach (var item in request.BookItemsId) 
            {
                if (item.IsDigital)
                {
                    loan.AddDigitalBook(item.BookId, Guid.NewGuid().ToString(), request.LoanDays);
                }
                else
                {
                    var bookItem = await _bookRepository.GetAvailableItemAsync(item.BookId, item.Barcode);

                    if (bookItem == null)
                        throw new Exception($"Sách với Barcode {item.Barcode} hiện không sẵn sàng để mượn.");
                    loan.AddPhysicalItem(bookItem, request.LoanDays);
                }
            }

            await _loanRepository.AddAsync(loan);
            await _unitOfWork.SaveChangesAsync();

            var loanWithData = await _loanRepository.GetByIdAsync(loan.Id);
            return _mapper.Map<LoanResponse>(loanWithData);
        }
        public async Task<LoanResponse> CreateLoanAsync(CreateLoanRequest request)
        {
            var reader = await _readerRepository.GetByIdAsync(request.ReaderId);
            var staff = await _staffRepository.GetByIdAsync(request.StaffId);

            if (reader == null) throw new EntityNotFoundException("Reader", request.ReaderId);
            if (staff == null) throw new EntityNotFoundException("Staff", request.StaffId);

            var loan = new Loan(reader, staff);

            foreach (var item in request.BookItemsId)
            {
                if (item.IsDigital)
                {
                    var book = await _bookRepository.GetByIdAsync(item.BookId);
                    if (book == null || string.IsNullOrEmpty(book.DigitalFilePath))
                        throw new DomainException($"Digital Book {item.BookId} not available.");

                    DateTime dueDate = DateTime.Now.AddDays(request.LoanDays);
                    string secureToken = _secureTokenService.GenerateEBookToken(reader.Id, book.DigitalFilePath, dueDate);

                    loan.AddDigitalBook(item.BookId, secureToken, request.LoanDays);

                    //var accessToken = Guid.NewGuid().ToString();
                    //loan.AddDigitalBook(item.BookId, accessToken, request.LoanDays);
                }
                else
                {
                    var bookItem = await _bookRepository.GetAvailableItemAsync(item.BookId, item.Barcode);

                    if (bookItem == null)
                        throw new DomainException($"Book with Barcode {item.Barcode} is not available or does not exist.");

                    loan.AddPhysicalItem(bookItem, request.LoanDays);

                    var readyReservation = await _reservationRepository.GetReadyReservationByBookItemAsync(bookItem.Id);
                    if (readyReservation != null)
                    {
                        readyReservation.Complete();
                        _reservationRepository.Update(readyReservation);
                    }
                }
            }

            await _loanRepository.AddAsync(loan);
            await _unitOfWork.SaveChangesAsync();
            //var loanWithData = await _loanRepository.GetByIdAsync(loan.Id);
            return _mapper.Map<LoanResponse>(loan);
        }

        public async Task UpdateStatusAsync(UpdateLoanStatusRequest request)
        {
            var detail = await _loanRepository.GetDetailByIdAsync(request.LoanDetailId);
            if (detail == null) throw new EntityNotFoundException("LoanDetail", request.LoanDetailId);

            detail.UpdateStatus(request.NewLoanStatus);

            if (detail.BookItem != null)
            {
                switch (request.NewBookStatus)
                {
                    case BookStatus.Available:
                        detail.BookItem.MarkAsAvailable();
                        break;
                    case BookStatus.Lost:
                        detail.BookItem.MarkAsLost();
                        break;
                    case BookStatus.InRepair:
                        detail.BookItem.MarkAsInRepair(); 
                        break;
                }
            }

            // Nếu trả sách bình thường, xử lý logic ngày trả và phạt
            if (request.NewLoanStatus == LoanStatus.Returned)
            {
                detail.ProcessReturn(dailyFineRate: 5000);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task ReturnPhysicalBookAsync(Guid loanDetailId, Guid staffId)
        {
            var detail = await _loanRepository.GetDetailByIdAsync(loanDetailId);
            if (detail == null) throw new Exception("Không tìm thấy thông tin mượn.");

            if (detail.AccessToken != null)
                throw new Exception("Sách số không thể trả tại quầy theo cách này.");

            detail.ProcessReturn(dailyFineRate: 5000);

            if (detail.BookItem != null)
            {
                detail.BookItem.MarkAsAvailable();
            }

            var bookItem = await _bookRepository.GetBookItemByIdAsync(detail.BookItemId);
            if (bookItem != null)
            {
                bookItem.MarkAsAvailable();
            }

            var auditLog = new AuditLog(
                staffId,                               
                _currentUserService.Username,           
                "Return Physical Book",                 
                "LoanDetails",                          
                detail.Id.ToString(),                   
                "Status: Loaned",                       
                $"Librarian successfull return the loan, Money: {detail.FineAmount.Amount} VND"
            );

            await _auditRepository.AddAsync(auditLog);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReturnBookAsync(Guid loanDetailId, Guid currentUserId)
        {
            var detail = await _loanRepository.GetDetailByIdAsync(loanDetailId);
            if (detail == null) throw new EntityNotFoundException("LoanDetail", loanDetailId);

            if (!string.IsNullOrEmpty(detail.AccessToken))
            {
                detail.ReturnBook(Money.Zero());
            }
            else
            {
                detail.BookItem?.MarkAsAvailable(); 
                detail.ProcessReturn(dailyFineRate: 5000);
                if (detail.FineAmount.Amount > 0)
                {
                    await _paymentService.CreateFineAsync(detail.Id, detail.FineAmount.Amount);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReturnDigitalBookAsync(Guid loanDetailId, Guid readerId)
        {
            var detail = await _loanRepository.GetDetailByIdAsync(loanDetailId);

            var loan = await _loanRepository.GetByIdAsync(detail.LoanId);
            if (loan.ReaderId != readerId) throw new Exception("Bạn không có quyền trả sách này.");

            if (string.IsNullOrEmpty(detail.AccessToken))
                throw new Exception("Đây không phải là sách số.");

            detail.ReturnBook(Money.Zero());

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReturnBookAsync(Guid loanDetailId)
        {
            var detail = await _unitOfWork.LoanDetails.GetByIdAsync(loanDetailId);
            if (detail == null) throw new Exception("Cant find loan detail.");

            var bookItem = await _unitOfWork.BookItems.GetByIdAsync(detail.BookItemId);
            if (bookItem != null)
            {
                bookItem.MarkAsAvailable();
            }

            detail.ProcessReturn(5000);

            if (detail.FineAmount.Amount > 0)
            {
                await _paymentService.CreateFineAsync(detail.Id, detail.FineAmount.Amount);
            }

            var reservation = await _reservationRepository.GetNextPendingReservationAsync(detail.BookItemId);
            if (reservation != null)
            {
                reservation.NotifyAvailable();
                _reservationRepository.Update(reservation);
            }

            var loan = await _loanRepository.GetByIdAsync(detail.LoanId);
            var reader = await _readerRepository.GetByIdAsync(loan.ReaderId);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<LoanResponse>> GetReaderLoanHistoryAsync(Guid accountId)
        {
            var reader = await _readerRepository.GetByAccountIdAsync(accountId);
            if (reader == null) return Enumerable.Empty<LoanResponse>();

            var loans = await _loanRepository.GetByReaderIdAsync(reader.Id);
            return _mapper.Map<IEnumerable<LoanResponse>>(loans);
        }

        public async Task ExtendLoanAsync(Guid loanDetailId, int extraDays)
        {
            var detail = await _loanRepository.GetDetailByIdAsync(loanDetailId);
            if (detail == null) throw new EntityNotFoundException("LoanDetail", loanDetailId);

            if (detail.Status != LoanStatus.Active)
                throw new DomainException("Chỉ có thể gia hạn sách đang trong trạng thái mượn.");

            if (detail.IsOverdue())
                throw new DomainException("Sách đã quá hạn, không thể tự gia hạn online. Vui lòng đến thư viện.");

            var readyRes = await _reservationRepository.GetReadyReservationByBookItemAsync(detail.BookItemId);
            if (readyRes != null)
                throw new DomainException("Không thể gia hạn vì đã có người khác đặt trước sách này.");

            detail.ExtendDueDate(extraDays);

            await _auditRepository.AddAsync(new AuditLog(
                _currentUserService.UserId, _currentUserService.Username,
                "Extend Loan", "LoanDetail", detail.Id.ToString(),
                "Success", $"Extended to {detail.DueDate:dd/MM/yyyy}")
            );

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
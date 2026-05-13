using AutoMapper;
using LibraryApplication.DTOs.Payments;
using LibraryApplication.Interfaces;
using LibraryDomain.Entities;
using LibraryDomain.Interfaces;
using LibraryDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryApplication.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;   
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepository, IAuditRepository auditRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateFineAsync(Guid loanDetailId, decimal amount)
        {
            var money = new LibraryDomain.ValueObjects.Money(amount, "VND");
            var fine = new FinePayment(loanDetailId, money);

            await _paymentRepository.AddAsync(fine);

            await _auditRepository.AddAsync(new AuditLog(
                null, "System", "CreateFine", "FinePayment", fine.Id.ToString(), "", $"Amount: {amount}"));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ProcessPaymentAsync(ProcessPaymentRequest request)
        {
            var fine = await _paymentRepository.GetByIdAsync(request.PaymentId);
            if (fine == null) throw new EntityNotFoundException("Fine", request.PaymentId);

            if (fine.IsPaid) throw new DomainException("Payment already processed.");

            fine.MarkAsPaid(request.PaymentMethod);

            await _auditRepository.AddAsync(new AuditLog(
                Guid.Empty,
                "Librarian_Name",
                "ProcessPayment",
                "FinePayment",
                fine.Id.ToString(),
                "Status: Unpaid",
                $"Status: Paid | Method: {request.PaymentMethod} | Note: {request.Note}"
            ));

            //_paymentRepository.Update(fine);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<PaymentResponse>> GetPendingFinesAsync(Guid readerId)
        {
            var fines = await _paymentRepository.GetByReaderIdAsync(readerId);

            return _mapper.Map<IEnumerable<PaymentResponse>>(fines);
        }
    }
}
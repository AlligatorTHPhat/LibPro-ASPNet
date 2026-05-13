using AutoMapper;
using LibraryApplication.DTOs.Readers;
using LibraryApplication.Interfaces;
using LibraryDomain.Entities;
using LibraryDomain.Enums;
using LibraryDomain.Exceptions;
using LibraryDomain.Interfaces;
using LibraryDomain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace LibraryApplication.Services
{
    public class ReaderService : IReaderService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditRepository _auditRepository;
        private readonly IReaderRepository _readerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReaderService(
            IAuditRepository auditRepository,
            IReaderRepository readerRepository,
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _auditRepository = auditRepository;
            _readerRepository = readerRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<ReaderResponse?> GetReaderByIdAsync(Guid id)
        {
            var reader = await _readerRepository.GetByIdAsync(id);
            if (reader == null) throw new EntityNotFoundException("Reader", id);

            return _mapper.Map<ReaderResponse>(reader);
        }

        public async Task<IEnumerable<ReaderResponse>> GetAllReadersAsync()
        {
            var readers = await _readerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReaderResponse>>(readers);
        }

        public async Task<ReaderResponse> CreateReaderAsync(CreateReaderRequest request)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new EntityNotFoundException("Account", request.AccountId);

            var existingReader = await _readerRepository.GetByAccountIdAsync(request.AccountId);
            if (existingReader != null)
                throw new DomainException("This account has already been registered with a reader profile.");

            if (!Enum.TryParse<ReaderType>(request.ReaderTypeName, true, out var readerTypeEnum))
            {
                readerTypeEnum = ReaderType.Guest;
            }

            var address = new Address(
                request.Street, request.Ward, request.District, request.City);
            var reader = new Reader(
                request.LibraryCardNumber,
                request.FullName,
                Gender.FromValue(request.Gender),
                request.DateOfBirth,
                address,
                request.PhoneNumber,
                request.AccountId,
                readerTypeEnum,
                false
            ); 

            await _readerRepository.AddAsync(reader);

            var userId = _currentUserService.UserId;
            var username = _currentUserService.Username;
            
            //var userId = user?.FindFirst("UserId")?.Value;
            //var username = user?.FindFirst(ClaimTypes.Name)?.Value;

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Create Reader",
                "Reader",
                "Create",
                string.Empty,
                $"Reader {reader.FullName} created at {DateTime.Now}"
            );

            await _auditRepository.AddAsync(auditLog);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReaderResponse>(reader);
        }

        public async Task UpdateReaderProfileAsync(Guid id, UpdateReaderRequest request)
        {
            var reader = await _readerRepository.GetByIdAsync(id);
            if (reader == null) throw new EntityNotFoundException("Reader", id);

            var newAddress = new LibraryDomain.ValueObjects.Address(
                request.Street, request.Ward, request.District, request.City);

            reader.UpdateProfile(request.FullName, newAddress, request.PhoneNumber, request.IsDeleted);

            _readerRepository.Update(reader);

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Update Reader",
                "System",
                "Auth",
                string.Empty,
                $"Reader profile updated at {DateTime.Now}"
            );
            await _auditRepository.AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteReaderAsync(Guid id)
        {
            var reader = await _readerRepository.GetByIdAsync(id);
            if (reader == null) throw new EntityNotFoundException("Reader", id);

            if (reader.HasOverdueLoans())
                throw new DomainException("Cannot delete reader with overdue books.");

            _readerRepository.Delete(reader);

            var account = await _accountRepository.GetByIdAsync(reader.AccountId);
            if (account != null)
            {
                account.SetStatus(true);
                _accountRepository.Update(account);
            }

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Delete Reader",
                "System",
                "Auth",
                string.Empty,
                $"Reader deleted at {DateTime.Now}"
            );

            await _auditRepository.AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreReaderAsync(Guid id)
        {
            var reader = await _readerRepository.GetByIdAsync(id);
            if (reader == null) throw new EntityNotFoundException("Reader", id);

            reader.UpdateProfile(reader.FullName, reader.Address, reader.PhoneNumber, false);
            _readerRepository.Update(reader);

            var account = await _accountRepository.GetByIdAsync(reader.AccountId);
            if (account != null)
            {
                account.SetStatus(false);
                _accountRepository.Update(account);
            }

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Restore Reader",
                "System",
                "Auth",
                string.Empty,
                $"Reader deleted at {DateTime.Now}"
            );

            await _auditRepository.AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> CanReaderBorrowAsync(Guid readerId)
        {
            var reader = await _readerRepository.GetByIdAsync(readerId);

            if (reader == null || reader.IsDeleted) return false;

            if (!reader.IsMembershipActive()) return false;

            var isEligible = await _readerRepository.IsEligibleForLoanAsync(readerId);

            return isEligible;
        }
        public async Task<bool> HasReaderProfileAsync(Guid accountId)
        {
            var reader = await _readerRepository.GetByAccountIdAsync(accountId);
            return reader != null;
        }
    }
}
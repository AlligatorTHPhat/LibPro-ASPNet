using AutoMapper;
using LibraryApplication.DTOs.Accounts;
using LibraryApplication.DTOs.Audits;
using LibraryApplication.Interfaces;
using LibraryDomain.Entities;
using LibraryDomain.Enums;
using LibraryDomain.Exceptions;
using LibraryDomain.Interfaces;
using LibraryDomain.ValueObjects;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Services
{
    public class AccountService : IAccountService
    {
        private readonly ICurrentUserService _currentUserService; 
        private readonly IReaderRepository _readerRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(ICurrentUserService currentUserService, IAccountRepository accountRepository, IReaderRepository readerRepository, IStaffRepository staffRepository, IAuditRepository auditRepository, ITokenService tokenService, IPasswordHasher passwordHasher, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _accountRepository = accountRepository;
            _auditRepository = auditRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _readerRepository = readerRepository;
            _staffRepository = staffRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<AccountResponse> RegisterReaderAsync(RegisterReaderRequest request)
        {
            if (await _accountRepository.GetByUsernameAsync(request.Username) != null)
                throw new EntityAlreadyExistsException("Username", request.Username);

            if (await _accountRepository.GetByEmailAsync(request.Email) != null)
                throw new EntityAlreadyExistsException("Email", request.Email);

            var account = new Account(
                request.Username,
                _passwordHasher.Hash(request.Password),
                request.Email,
                UserRole.Reader,
                false
            ); await _accountRepository.AddAsync(account);

            if (!Enum.TryParse<ReaderType>(request.Type.ToString(), true, out var readerTypeEnum))
            {
                readerTypeEnum = ReaderType.Guest;
            }

            var reader = new Reader(
                $"LCN-{DateTime.Now.Ticks}",
                request.FullName,
                Gender.FromValue(request.Gender),
                request.DateOfBirth,
                new Address(request.Street, request.Ward, request.District, request.City),
                request.PhoneNumber,
                account.Id,
                request.Type,
                false
            ); await _readerRepository.AddAsync(reader);

            var auditLog = new AuditLog(
                account.Id,
                account.Username,
                "Self Register", "Reader", "Register",
                string.Empty, $"New reader {reader.FullName} registered self."
            ); await _auditRepository.AddAsync(auditLog);

            await _unitOfWork.CompleteAsync();

            var token = _tokenService.GenerateToken(account);

            return new AccountResponse
            {
                Token = token,
                Username = account.Username,
                Role = account.Role.ToString()
            };
        }

        public async Task<AccountResponse> RegisterLibrarianAsync(RegisterStaffRequest request)
        {
            if (await _accountRepository.GetByUsernameAsync(request.Username) != null)
                throw new EntityAlreadyExistsException("Username", request.Username);

            if (await _accountRepository.GetByEmailAsync(request.Email) != null)
                throw new EntityAlreadyExistsException("Email", request.Email);

            var account = new Account(
                request.Username,
                _passwordHasher.Hash(request.Password),
                request.Email,
                UserRole.Librarian,
                false
            );

            var staff = new Staff(
                $"LIB-{DateTime.Now.Ticks}",
                request.FullName,
                Gender.FromValue(request.Gender),
                request.DateOfBirth,
                new Address(request.Street, request.Ward, request.District, request.City),
                request.PhoneNumber,
                account.Id,
                false
            );

            await _unitOfWork.Accounts.AddAsync(account);
            await _staffRepository.AddAsync(staff);

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Register/Create Librarian",
                "System",
                "Auth",
                string.Empty,
                $"Librarian registered/ created at {DateTime.Now}"
            );

            await _auditRepository.AddAsync(auditLog);
            await _unitOfWork.CompleteAsync();

            var token = _tokenService.GenerateToken(account);
            return new AccountResponse
            {
                Token = token,
                Username = account.Username,
                Role = account.Role.ToString()
            };
        }
        public async Task<AccountResponse?> LoginAsync(LoginRequest request)
        {
            var account = await _accountRepository.GetByUsernameAsync(request.Username);

            if (account == null || account.IsDeleted)
                throw new DomainException("User not found or locked!");

            var isValid = _passwordHasher.Verify(request.Password, account.PasswordHash);

            if (!isValid)
                throw new DomainException("Invalid password");

            var token = _tokenService.GenerateToken(account);

            return new AccountResponse
            {
                Token = token,
                Username = account.Username,
                Role = account.Role.ToString(),
                Id = account.Id
            };
        }

        /*Sau này search by name*/

        public async Task<AccountResponse?> GetAccountByIdAsync(Guid id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null) throw new EntityNotFoundException("Account", id);

            var response = _mapper.Map<AccountResponse>(account);

            if (account.Role == UserRole.Reader) // Giả sử UserRole là Enum
            {
                var reader = await _readerRepository.GetByAccountIdAsync(id);
                response.FullName = reader?.FullName ?? "N/A";
            }
            else if (account.Role == UserRole.Director || account.Role == UserRole.Librarian)
            {
                var staff = await _staffRepository.GetByAccountIdAsync(id);
                response.FullName = staff?.FullName ?? "N/A";
            }

            return response;
        }

        public async Task<IEnumerable<AccountResponse>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllActiveAsync();
            return _mapper.Map<IEnumerable<AccountResponse>>(accounts);


        }

        public async Task ChangePasswordAsync(Guid id, string oldPassword, string newPassword)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null) throw new EntityNotFoundException("Account", id);

            if (!_passwordHasher.Verify(oldPassword, account.PasswordHash))
                throw new DomainException("Wrong old password.");

            account.UpdatePassword(newPassword);
            
            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Update Password",
                "System",
                "Auth",
                string.Empty,
                $"User updated password at {DateTime.Now}"
            );
            await _auditRepository.AddAsync(auditLog);

            _accountRepository.Update(account);
        }

        public async Task ToggleAccountStatusAsync(Guid id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null) throw new EntityNotFoundException("Account", id);

            account.SetStatus(account.IsDeleted);
            _accountRepository.Update(account);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
            => await _accountRepository.GetByUsernameAsync(username) == null;

        public async Task<bool> IsEmailUniqueAsync(string email)
            => await _accountRepository.GetByEmailAsync(email) == null;

        public async Task DeleteAccountAsync(Guid id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null) throw new EntityNotFoundException("Account", id);

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Delete Account",
                "System",
                "Auth",
                string.Empty,
                $"User deleted at {DateTime.Now}"
            );
            await _auditRepository.AddAsync(auditLog);

            account.SetStatus(true);
            _accountRepository.Update(account);
        }
        public async Task<bool> IsEligibleAsync(Guid readerId)
        {
            var reader = await _readerRepository.GetByIdAsync(readerId);
            // Thẻ hết hạn thì không được mượn
            if (reader == null || !reader.IsMembershipActive()) return false;
            return await _readerRepository.IsEligibleForLoanAsync(readerId);
        }
    }
}
using AutoMapper;
using LibraryApplication.DTOs.Staffs;
using LibraryApplication.Interfaces;
using LibraryDomain.Entities;
using LibraryDomain.Enums;
using LibraryDomain.Exceptions;
using LibraryDomain.Interfaces;
using LibraryDomain.ValueObjects;

namespace LibraryApplication.Services
{
    public class StaffService : IStaffService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStaffRepository _staffRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StaffService(
            IAuditRepository auditRepository,
            ICurrentUserService currentUserService,
            IStaffRepository staffRepository,
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _auditRepository = auditRepository;
            _currentUserService = currentUserService;
            _staffRepository = staffRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<StaffResponse> CreateStaffAsync(CreateStaffRequest request)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new EntityNotFoundException("Account", request.AccountId);

            if (account.Role == UserRole.Director || account.Role == UserRole.Reader)
                throw new DomainException("This account has the role of Director or Reader and cannot be used to create a staff profile.");

            var existingStaff = await _staffRepository.GetByAccountIdAsync(request.AccountId);
            if (existingStaff != null)
                throw new DomainException("This account has already been linked with another staff profile.");

            var gender = request.Gender switch
            {
                "Male" => Gender.Male,
                "Female" => Gender.Female,
                _ => throw new Exception("Invalid gender")
            };
            var address = new Address(request.Street, request.Ward, request.District, request.City);
            var staffCode = $"STF{DateTime.Now.ToString("yyyyMMddHHmm")}";
            var staff = new Staff(
                staffCode,
                request.FullName,
                gender,
                request.DateOfBirth,
                address,
                request.PhoneNumber,
                request.AccountId,
                false
            );

            await _staffRepository.AddAsync(staff);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<StaffResponse>(staff);
        }

        public async Task<StaffResponse?> GetStaffByIdAsync(Guid id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null) throw new EntityNotFoundException("Staff", id);

            return _mapper.Map<StaffResponse>(staff);
        }

        public async Task<IEnumerable<StaffResponse>> GetAllStaffsAsync()
        {
            var staffs = await _staffRepository.GetByRoleAsync(UserRole.Librarian);
            return _mapper.Map<IEnumerable<StaffResponse>>(staffs);
        }

        public async Task UpdateStaffProfileAsync(Guid id, UpdateStaffRequest request)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null) throw new EntityNotFoundException("Staff", id);

            var newAddress = new LibraryDomain.ValueObjects.Address(
                request.Street, request.Ward, request.District, request.City);

            staff.UpdateInfo(request.FullName, newAddress, request.PhoneNumber, request.IsDeleted);
            _staffRepository.Update(staff);

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Update Staff",
                "System",
                "Auth",
                string.Empty,
                $"Staff profile updated at {DateTime.Now}"
            );
            await _auditRepository.AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteStaffAsync(Guid id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null) throw new EntityNotFoundException("Staff", id);

            if (staff.Account.Role == UserRole.Director)
                throw new DomainException("Cannot delete Director account from the system.");

            staff.Deactivate();
            _staffRepository.Update(staff);

            var account = await _accountRepository.GetByIdAsync(staff.AccountId);
            account.Deactivate();
            
            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Delete Staff",
                "System",
                "Auth",
                string.Empty,
                $"Staff deleted at {DateTime.Now}"
            );

            await _auditRepository.AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreStaffAsync(Guid id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null) throw new EntityNotFoundException("Staff", id);

            staff.Activate();
            _staffRepository.Update(staff);

            var account = await _accountRepository.GetByIdAsync(staff.AccountId);
            account.Activate();

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Restore Staff",
                "System",
                "Auth",
                string.Empty,
                $"Staff restored at {DateTime.Now}"
            );
            await _auditRepository.AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsAccountStaffAsync(Guid accountId)
        {
            var staff = await _staffRepository.GetByAccountIdAsync(accountId);
            return staff != null;
        }
    }
}
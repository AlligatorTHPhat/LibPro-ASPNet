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
        private readonly IStaffRepository _staffRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StaffService(
            IStaffRepository staffRepository,
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
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
            var staffs = await _staffRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<StaffResponse>>(staffs);
        }

        public async Task UpdateStaffProfileAsync(Guid id, UpdateStaffRequest request)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null) throw new EntityNotFoundException("Staff", id);

            // Tạo Value Object Address mới từ Request
            var newAddress = new LibraryDomain.ValueObjects.Address(
                request.Street, request.Ward, request.District, request.City);

            // Cập nhật thông qua phương thức của Entity (Domain Driven)
            staff.UpdateInfo(request.FullName, newAddress, request.PhoneNumber, request.IsDeleted);
            _staffRepository.Update(staff);
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
            if (account != null)
            {
                account.SetStatus(true);
                _accountRepository.Update(account);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreStaffAsync(Guid id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null) throw new EntityNotFoundException("Staff", id);

            staff.Activate();
            _staffRepository.Update(staff);

            var account = await _accountRepository.GetByIdAsync(staff.AccountId);
            if (account != null)
            {
                account.SetStatus(false);
                _accountRepository.Update(account);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsAccountStaffAsync(Guid accountId)
        {
            var staff = await _staffRepository.GetByAccountIdAsync(accountId);
            return staff != null;
        }
    }
}
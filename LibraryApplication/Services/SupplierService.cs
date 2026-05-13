using AutoMapper;
using LibraryApplication.DTOs;
using LibraryApplication.Interfaces;
using LibraryDomain.Entities;
using LibraryDomain.Interfaces;

namespace LibraryApplication.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SupplierService(ICurrentUserService currentUserService, IAuditRepository auditRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _currentUserService = currentUserService;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task <SupplierCreateDto> CreateAsync(SupplierCreateDto dto)
        {
            var supplier = new Supplier(dto.Name, dto.ContactPerson, dto.Email)
            {
                TaxCode = dto.TaxCode,
                PhoneNumber = dto.PhoneNumber,
                OfficeAddress = dto.OfficeAddress
            };

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Create Supplier",
                "SupplierManagement",
                "Library",
                string.Empty,
                $"Supplier '{dto.Name}' created at {DateTime.Now}");
            await _auditRepository.AddAsync(auditLog);

            return _mapper.Map<SupplierCreateDto>(supplier);
        }

        public async Task <SupplierUpdateDto> UpdateAsync(SupplierUpdateDto dto)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.Id);
            if (supplier == null) throw new Exception("Supplier not found");

            supplier.UpdateSupplierDetails(dto.Name, dto.OfficeAddress ?? "", dto.PhoneNumber ?? "");
            supplier.TaxCode = dto.TaxCode;
            supplier.ContactPerson = dto.ContactPerson;
            supplier.Email = dto.Email;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Update Supplier",
                "SupplierManagement",
                "Library",
                string.Empty,
                $"Supplier ID: {dto.Id} updated at {DateTime.Now}");
            await _auditRepository.AddAsync(auditLog);

            return _mapper.Map<SupplierUpdateDto>(supplier);
        }
    }
}
using AutoMapper;
using LibraryApplication.DTOs;
using LibraryApplication.Interfaces;
using LibraryDomain.Entities;
using LibraryDomain.Interfaces;

namespace LibraryApplication.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PublisherService(ICurrentUserService currentUserService, IAuditRepository auditRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _currentUserService = currentUserService;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task <PublisherCreateDto> CreateAsync(PublisherCreateDto dto)
        {
            var publisher = new Publisher(dto.Name, dto.Email)
            {
                PhoneNumber = dto.PhoneNumber,
                OfficeAddress = dto.OfficeAddress
            };

            await _unitOfWork.Publishers.AddAsync(publisher);
            await _unitOfWork.SaveChangesAsync();

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Create Publisher",
                "PublisherManagement",
                "Library",
                string.Empty,
                $"Publisher '{dto.Name}' created at {DateTime.Now}");
            await _auditRepository.AddAsync(auditLog);

            return _mapper.Map<PublisherCreateDto>(publisher);

        }

        public async Task <PublisherUpdateDto> UpdateAsync(PublisherUpdateDto dto)
        {
            var publisher = await _unitOfWork.Publishers.GetByIdAsync(dto.Id);
            if (publisher == null) throw new Exception("Publisher not found");

            publisher.Name = dto.Name;
            publisher.UpdateContactInfo(dto.Email ?? "", dto.PhoneNumber ?? "");
            
            publisher.OfficeAddress = dto.OfficeAddress;

            _unitOfWork.Publishers.Update(publisher);
            await _unitOfWork.SaveChangesAsync();

            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                "Update Publisher",
                "PublisherManagement",
                "Library",
                string.Empty,
                $"Publisher ID: {dto.Id} updated at {DateTime.Now}");
            await _auditRepository.AddAsync(auditLog);

            return _mapper.Map<PublisherUpdateDto>(publisher);
        }
    }
}
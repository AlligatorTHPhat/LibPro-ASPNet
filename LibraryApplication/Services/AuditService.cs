using AutoMapper;
using LibraryApplication.DTOs.Audits;
using LibraryApplication.Interfaces;
using LibraryDomain.Exceptions;
using LibraryDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryApplication.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AuditService(IAuditRepository auditRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AuditResponse> GetLogByIdAsync(Guid id)
        {
            var log = await _unitOfWork.Audits.GetByIdAsync(id);

            if (log == null)
            {
                throw new EntityNotFoundException("AuditLog", id);
            }

            return _mapper.Map<AuditResponse>(log);
        }

        public async Task<IEnumerable<AuditResponse>> GetSystemHistoryAsync()
        {
            var logs = await _unitOfWork.Audits.GetAllAsync();
            return _mapper.Map<IEnumerable<AuditResponse>>(logs);
        }

        public async Task<IEnumerable<AuditResponse>> GetEntityHistoryAsync(string entityName, string entityId)
        {
            var logs = await _unitOfWork.Audits.GetByEntityAsync(entityName, entityId.ToString());
            return _mapper.Map<IEnumerable<AuditResponse>>(logs);
        }

        public async Task<IEnumerable<AuditResponse>> GetUserActivityAsync(Guid userId)
        {
            var logs = await _unitOfWork.Audits.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<AuditResponse>>(logs);
        }
    }
}
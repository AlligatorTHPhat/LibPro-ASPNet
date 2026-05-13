using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryApplication.DTOs.Readers;

namespace LibraryApplication.Interfaces
{
    public interface IReaderService
    {
        Task<ReaderResponse?> GetReaderByIdAsync(Guid id);
        Task<IEnumerable<ReaderResponse>> GetAllReadersAsync();

        Task<ReaderResponse> CreateReaderAsync(CreateReaderRequest request);
        Task UpdateReaderProfileAsync(Guid id, UpdateReaderRequest request);
        Task DeleteReaderAsync(Guid id);
        Task RestoreReaderAsync(Guid id);
        Task<bool> HasReaderProfileAsync(Guid accountId);
        Task<bool> CanReaderBorrowAsync(Guid readerId);
    }
}
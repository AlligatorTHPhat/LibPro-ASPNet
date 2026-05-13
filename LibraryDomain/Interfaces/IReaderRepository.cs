using LibraryDomain.Entities;
using LibraryDomain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryDomain.Interfaces
{
    public interface IReaderRepository
    {
        Task<Reader?> GetByIdAsync(Guid id);
        Task<Reader?> GetByCardNumberAsync(string cardNumber);
        Task<Reader?> GetByAccountIdAsync(Guid accountId);

        Task<IEnumerable<Reader>> GetAllActiveAsync();
        Task AddAsync(Reader reader);
        void Update(Reader reader);
        void Delete(Reader reader); 
        Task<bool> IsEligibleForLoanAsync(Guid readerId);



        Task<IEnumerable<Reader>> GetAllAsync();
        Task<IEnumerable<Reader>> SearchAsync(string searchTerm);
        Task<IEnumerable<Reader>> GetByTypeAsync(ReaderType type);
        Task<IEnumerable<Reader>> GetExpiredReadersAsync();

        Task<bool> ExistsByCardNumberAsync(string cardNumber);

        Task<int> CountAsync();
        Task<List<Guid>> GetTopRiskReaderIdsAsync(int top);

        //Task<IEnumerable<ReaderRankingDto>> GetTopRiskReadersAsync(int top);
    }
}
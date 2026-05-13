using System;
using System.Collections.Generic;
using System.Text;
using LibraryDomain.Entities;

namespace LibraryDomain.Interfaces
{
    public interface IPublisherRepository
    {
        Task<Publisher?> GetByIdAsync(Guid id);
        Task<IEnumerable<Publisher>> GetAllAsync();
        Task AddAsync(Publisher publisher);
        void Update(Publisher publisher);
    }   
}

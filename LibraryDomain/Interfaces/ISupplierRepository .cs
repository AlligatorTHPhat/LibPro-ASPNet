using System;
using System.Collections.Generic;
using System.Text;
using LibraryDomain.Entities;

namespace LibraryDomain.Interfaces
{
    public interface ISupplierRepository
    {
        Task<Supplier?> GetByIdAsync(Guid id);
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task AddAsync(Supplier supplier);
        void Update(Supplier supplier);
    }
}

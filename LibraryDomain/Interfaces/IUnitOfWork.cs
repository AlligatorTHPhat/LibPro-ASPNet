using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryDomain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository Accounts { get; }
        IAuditRepository Audits { get; }
        IBookRepository Books { get; }
        IGenericRepository<BookItem> BookItems { get; }
        ICategoryRepository Categories { get; }
        ILoanRepository Loans { get; }
        ILoanDetailRepository LoanDetails { get; }
        IReaderRepository Readers { get; }
        IStaffRepository Staffs { get; }
        IPaymentRepository Payments { get; }
        IPublisherRepository Publishers { get; }
        ISupplierRepository Suppliers { get; }
        IReservationRepository Reservations { get; }

        Task<int> CompleteAsync();
        Task<int> SaveChangesAsync();

        //Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

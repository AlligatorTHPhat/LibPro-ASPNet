using LibraryDomain.Entities;
using LibraryDomain.Interfaces;
using LibraryInfrastructure.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext _context;

        private IBookRepository? _books;
        private IGenericRepository<BookItem?> _bookItems;
        private ICategoryRepository? _categories;
        private IReaderRepository? _readers;
        private IAccountRepository? _accounts;
        private IAuditRepository? _audits;
        private ILoanRepository? _loans;
        private ILoanDetailRepository? _loanDetails;
        private IStaffRepository? _staffs;
        private IPaymentRepository? _payment;
        private IPublisherRepository? _publisher;
        private ISupplierRepository? _suppliers;
        private IReservationRepository? _reservation;

        public IBookRepository Books => _books ??= new BookRepository(_context);
        public IGenericRepository<BookItem> BookItems => _bookItems ??= new GenericRepository<BookItem>(_context);
        public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
        public IReaderRepository Readers => _readers ??= new ReaderRepository(_context);
        public IAccountRepository Accounts => _accounts ??= new AccountRepository(_context);
        public IAuditRepository Audits => _audits ??= new AuditRepository(_context);
        public ILoanRepository Loans => _loans ??= new LoanRepository(_context);
        public IStaffRepository Staffs => _staffs ??= new StaffRepository(_context);
        public ISupplierRepository Suppliers => _suppliers ??= new SupplierRepository(_context);
        public IPaymentRepository Payments => _payment ??= new PaymentRepository(_context);
        public IPublisherRepository Publishers => _publisher ??= new PublisherRepository(_context);
        public IReservationRepository Reservations => _reservation ??= new ReservationRepository(_context);
        public ILoanDetailRepository LoanDetails => _loanDetails ??= new LoanDetailRepository(_context);

        public UnitOfWork(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
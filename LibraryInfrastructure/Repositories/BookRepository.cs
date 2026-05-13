using LibraryDomain.Entities;
using LibraryDomain.Enums;
using LibraryDomain.Interfaces;
using LibraryDomain.ValueObjects;
using LibraryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LibraryInfrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;

        public DbSet<Book> Books { get; set; }

        public BookRepository(LibraryDbContext context) => _context = context;

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.BookItems)
                .IgnoreQueryFilters()
                .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(Guid id) =>
            await _context.Books.Include(b => b.BookItems.Where(bi => !bi.IsDeleted)).FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

        public async Task<Book?> GetByIsbnAsync(string isbn) =>
            await _context.Books.FirstOrDefaultAsync(b => (string)(object)b.Isbn == isbn);
        
        public async Task<Book?> GetByIdIncludeDeletedAsync(Guid id) =>
            await _context.Books.IgnoreQueryFilters().Include(b => b.BookItems).FirstOrDefaultAsync(b => b.Id == id);

        public async Task<IEnumerable<Book>> GetAllActiveAsync() =>
            await _context.Books.Where(b => !b.IsDeleted).ToListAsync();

        public async Task<IEnumerable<Book>> SearchByTitleAsync(string title) =>
            await _context.Books.Where(b => b.Title.Contains(title) && !b.IsDeleted).ToListAsync();

        public async Task<IEnumerable<Book>> SearchByAuthorAsync(string author) =>
            await _context.Books.Where(b => b.Author!.Contains(author) && !b.IsDeleted).ToListAsync();

        public async Task AddAsync(Book book) => await _context.Books.AddAsync(book);
        public void Update(Book book) => _context.Books.Update(book);
        
        // --- Book Item Implementation ---
        public async Task<IEnumerable<BookItem>> GetAllBookItemsAsync() =>
            await _context.BookItems.Include(bi => bi.Book).ToListAsync();

        public async Task<BookItem?> GetBookItemByIdAsync(Guid id) =>
            await _context.BookItems.FirstOrDefaultAsync(bi => bi.Id == id && !bi.IsDeleted);

        public async Task<BookItem?> GetAvailableItemAsync(Guid bookId, string barcode) =>
            await _context.BookItems.FirstOrDefaultAsync(bi =>
                bi.BookId == bookId &&
                bi.Barcode == barcode &&
                bi.Status == LibraryDomain.Enums.BookStatus.Available &&
                !bi.IsDeleted);
        public async Task<BookItem?> GetBookItemByIdIncludeDeletedAsync(Guid id) =>
            await _context.BookItems.IgnoreQueryFilters().FirstOrDefaultAsync(bi => bi.Id == id);

        public async Task<BookItem?> GetBookItemWithDetailsAsync(Guid id) => await _context.BookItems.Include(bi => bi.Book).ThenInclude(b => b.Category).FirstOrDefaultAsync(bi => bi.Id == id);

        public async Task<LoanDetail?> GetActiveLoanDetailByBookItemIdAsync(Guid bookItemId) =>
            await _context.LoanDetails.FirstOrDefaultAsync(ld => ld.BookItemId == bookItemId && ld.Status == LoanStatus.Active);

        public async Task<string?> GetLastBarcodeByIsbnAsync(string isbn)
        {
            return await _context.BookItems
                .IgnoreQueryFilters()
                .Where(bi => bi.Barcode.StartsWith(isbn))
                .OrderByDescending(bi => bi.Barcode)
                .Select(bi => bi.Barcode)
                .FirstOrDefaultAsync();
        }

        public async Task AddBookItemAsync(BookItem bookItem) => await _context.BookItems.AddAsync(bookItem);
        public async Task UpdateBookItemStatusAsync(Guid bookItemId, BookStatus status)
        {
            var bookItem = await _context.BookItems.FindAsync(bookItemId);
        }
        public async Task DeleteBookItemAsync(BookItem bookItem) => _context.BookItems.Update(bookItem);
        public async Task RestoreBookItemAsync(BookItem bookItem) => _context.BookItems.Update(bookItem);

        public async Task<int> CountByStatusAsync(BookStatus status) => await _context.BookItems.CountAsync(bi => bi.Status == status);
 

        public async Task<int> CountTotalCopiesAsync() => await _context.BookItems.CountAsync(bi => !bi.IsDeleted);

        public async Task<bool> HasAvailableCopyAsync(Guid bookId) =>
            await _context.BookItems.AnyAsync(bi => bi.BookId == bookId && bi.Status == LibraryDomain.Enums.BookStatus.Available && !bi.IsDeleted);
    }
}
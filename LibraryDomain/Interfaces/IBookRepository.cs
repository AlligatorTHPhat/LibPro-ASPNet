using LibraryDomain.Entities;
using LibraryDomain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryDomain.Interfaces
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        // Book
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(Guid id);
        Task<Book?> GetByIsbnAsync(string isbn);
        Task<Book?> GetByIdIncludeDeletedAsync(Guid id);
        Task<IEnumerable<Book>> GetAllActiveAsync();
        Task<IEnumerable<Book>> SearchByTitleAsync(string title);
        Task<IEnumerable<Book>> SearchByAuthorAsync(string author);

        Task AddAsync(Book book);
        void Update(Book book);

        // Book Items
        Task<IEnumerable<BookItem>> GetAllBookItemsAsync();
        Task<BookItem?> GetBookItemByIdAsync(Guid id);
        Task<BookItem?> GetAvailableItemAsync(Guid bookId, string barcode);
        Task<BookItem?> GetBookItemByIdIncludeDeletedAsync(Guid id);
        Task<BookItem?> GetBookItemWithDetailsAsync(Guid id);
        Task<LoanDetail?> GetActiveLoanDetailByBookItemIdAsync(Guid bookItemId);
        Task<string?> GetLastBarcodeByIsbnAsync(string isbn);
        Task AddBookItemAsync(BookItem bookItem);
        Task UpdateBookItemStatusAsync(Guid bookItemId, BookStatus status);
        Task DeleteBookItemAsync(BookItem bookItem);
        Task RestoreBookItemAsync(BookItem bookItem);

        // Stats
        Task<int> CountByStatusAsync(BookStatus status);
        Task<int> CountTotalCopiesAsync();
        Task<bool> HasAvailableCopyAsync(Guid bookId);
    }
}
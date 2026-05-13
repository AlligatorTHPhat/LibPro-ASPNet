using LibraryApplication.DTOs;
using LibraryDomain.Entities;
using LibraryDomain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryApplication.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookResponse>> GetAllBooksAsync();
        Task<BookResponse?> GetBookByIdAsync(Guid id);
        Task<BookResponse?> GetDeletedBookByIdAsync(Guid id); 
        Task<IEnumerable<BookResponse>> SearchBookByTitleAsync(string title); 
        Task<IEnumerable<BookResponse>> SearchBookByAuthorAsync(string author); 

        Task<BookResponse> CreateBookAsync(CreateBookRequest request);
        Task UpdateBookAsync(Guid id, UpdateBookRequest request);
        Task<BookResponse?> GetBookDetailsWithDeletedItemsAsync(Guid id);
        Task AddBookItemAsync(BookItemRequest request);
        Task UpdateBookItemStatusAsync(Guid bookItemId, LoanStatus Status, BookStatus bookStatus);
        Task DeleteBookItemAsync(Guid bookItemId);
        Task RestoreBookItemAsync(Guid bookItemId);

        Task<string> SaveImage(IFormFile imageFile);
    }
}
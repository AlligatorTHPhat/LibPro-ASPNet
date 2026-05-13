using AutoMapper;
using LibraryApplication.DTOs;
using LibraryApplication.Interfaces;
using LibraryDomain.Entities;
using LibraryDomain.Enums;
using LibraryDomain.Exceptions;
using LibraryDomain.Interfaces;
using LibraryDomain.ValueObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace LibraryApplication.Services
{
    public class BookService : IBookService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditRepository _auditRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPublisherRepository _publisherRepository;
        private readonly ISupplierRepository _supplierRepository;
        public BookService(
            IBookRepository bookRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditRepository auditRepository,
            ICurrentUserService currentUserService,
            IPublisherRepository publisherRepository,
            ISupplierRepository supplierRepository,
            ICategoryRepository categoryRepository  
            )
        {
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditRepository = auditRepository;
            _currentUserService = currentUserService;
            _publisherRepository = publisherRepository;
            _supplierRepository = supplierRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<BookResponse>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            var response = _mapper.Map<IEnumerable<BookResponse>>(books);

            foreach (var bookRes in response)
            {
                var bookEntity = books.FirstOrDefault(b => b.Id == bookRes.Id);
                if (bookEntity != null && bookEntity.BookItems != null)
                {
                    bookRes.TotalCopies = bookEntity.BookItems.Count();
                    bookRes.AvailableCopies = bookEntity.BookItems.Count(bi =>
                        !bi.IsDeleted && bi.Status == LibraryDomain.Enums.BookStatus.Available);
                }
            }

            return response;
        }

        public async Task<BookResponse?> GetBookByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return _mapper.Map<BookResponse>(book);
        }

        public async Task<BookResponse> CreateBookAsync(CreateBookRequest request)
        {
            var existingBook = await _bookRepository.GetByIsbnAsync(request.Isbn);
            if (existingBook != null)
                throw new DomainException($"Book with ISBN {request.Isbn} is already exists.");

            var publisher = await _publisherRepository.GetByIdAsync(request.PublisherId)
                ?? throw new EntityNotFoundException("Publisher", request.PublisherId);

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId)
                ?? throw new EntityNotFoundException("Category", request.CategoryId);

            var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId)
                ?? throw new EntityNotFoundException("Supplier", request.SupplierId);

            var isbn = new Isbn(request.Isbn);
            var book = new Book(request.Title, new Isbn(request.Isbn), publisher, supplier, category, false);

            if (request.ImageFile != null)
            {
                book.CoverImageUrl = await SaveImage(request.ImageFile);
            }

            book.Description = request.Description;
            book.Author = request.Author;


            if (request.IsDigital)
            {
                book.MarkAsDigital(request.DigitalUrl ?? "", DateTime.Now.AddYears(1));
            }
            else
            {
                for (int i = 1; i <= request.InitialCopies; i++)
                {
                    string barcode = $"{request.Isbn}-{i:D3}"; book.AddBookItem(barcode, request.DefaultShelf ?? "Kệ chờ");
                }
            }

            await _bookRepository.AddAsync(book);
            await _unitOfWork.SaveChangesAsync();
            await LogAction("Create Book Item", $"Created item with Id: {book.Id}");
            return _mapper.Map<BookResponse>(book);
        }

        public async Task UpdateBookAsync(Guid id, UpdateBookRequest request)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) throw new EntityNotFoundException("Book", id);

            if (request.ImageFile != null)
            {
                book.CoverImageUrl = await SaveImage(request.ImageFile);
            }

            _mapper.Map(request, book);

            _bookRepository.Update(book);
            await LogAction("Update Book", $"Updated book with ID: {book.Id}");
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<BookResponse?> GetDeletedBookByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdIncludeDeletedAsync(id);
            return _mapper.Map<BookResponse>(book);
        }

        public async Task<IEnumerable<BookResponse>> SearchBookByTitleAsync(string title)
        {
            var books = await _bookRepository.SearchByTitleAsync(title);
            return _mapper.Map<IEnumerable<BookResponse>>(books);
        }

        public async Task<IEnumerable<BookResponse>> SearchBookByAuthorAsync(string author)
        {
            var books = await _bookRepository.SearchByAuthorAsync(author);
            return _mapper.Map<IEnumerable<BookResponse>>(books);
        }

        public async Task<BookResponse?> GetBookDetailsWithDeletedItemsAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdIncludeDeletedAsync(id);
            if (book == null) return null;

            var response = _mapper.Map<BookResponse>(book);

            response.BookItems = book.BookItems.Select(bi => new BookItemResponse
            {
                Id = bi.Id,
                Barcode = bi.Barcode,
                Location = bi.ShelfLocation,
                Status = bi.Status.ToString(),
                IsDeleted = bi.IsDeleted
            }).ToList();

            response.TotalCopies = book.BookItems.Count(bi => !bi.IsDeleted);
            response.AvailableCopies = book.BookItems.Count(bi => !bi.IsDeleted && bi.Status == BookStatus.Available);

            return response;
        }

        public async Task AddBookItemAsync(BookItemRequest request)
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId);
            if (book == null) throw new Exception("Book not found.");

            string? lastBarcode = await _bookRepository.GetLastBarcodeByIsbnAsync(book.Isbn.Value);

            int nextSequence = 1;
            if (!string.IsNullOrEmpty(lastBarcode))
            {
                // Extract "003", parse to 3, and add 1
                var parts = lastBarcode.Split('-');
                if (parts.Length > 1 && int.TryParse(parts[1], out int lastNum))
                {
                    nextSequence = lastNum + 1;
                }
            }

            for (int i = 0; i < request.Quantity; i++)
            {
                string newBarcode = $"{book.Isbn.Value}-{nextSequence:D3}";
                var newItem = new BookItem(newBarcode, request.ShelfLocation, book.Id, false);

                await _bookRepository.AddBookItemAsync(newItem);
                nextSequence++;
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateBookItemStatusAsync(Guid bookItemId, LoanStatus loanStatus, BookStatus bookStatus)
        {
            var bookItem = await _bookRepository.GetBookItemByIdAsync(bookItemId);
            if (bookItem == null) throw new EntityNotFoundException("BookItem", bookItemId);

            if (bookItem.Status == BookStatus.Loaned || bookItem.Status == BookStatus.Reserved)
            {
                throw new DomainException("Cannot update status for a book item that is currently Loaned or Reserved. Please process return or cancellation first.");
            }

            switch (bookStatus)
            {
                case BookStatus.Available: bookItem.MarkAsAvailable(); break;
                case BookStatus.Loaned: bookItem.MarkAsLoaned(); break;
                case BookStatus.InRepair: bookItem.MarkAsInRepair(); break;
                case BookStatus.Lost: bookItem.MarkAsLost(); break;
                case BookStatus.Discarded: bookItem.MarkAsUnavaiable(); break;
                case BookStatus.Reserved: bookItem.MarkAsReserved(); break;
            }

            var loanDetail = await _bookRepository.GetActiveLoanDetailByBookItemIdAsync(bookItemId);
            if (loanDetail != null)
            {
                loanDetail.UpdateStatus(loanStatus);
            }

            await _auditRepository.AddAsync(new AuditLog(
                _currentUserService.UserId, _currentUserService.Username,
                "Update Book Status", "BookItem", bookItemId.ToString(),
                "Status Change", $"Changed to {bookStatus}"
            ));

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteBookItemAsync(Guid bookItemId)
        {
            var item = await _bookRepository.GetBookItemByIdAsync(bookItemId);
            if (item == null) throw new Exception("Book Item not found.");

            item.Delete();
            await _bookRepository.DeleteBookItemAsync(item);

            await LogAction("Delete Book Item", $"Deleted item with Barcode: {item.Barcode}");
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreBookItemAsync(Guid bookItemId)
        {
            var item = await _bookRepository.GetBookItemByIdIncludeDeletedAsync(bookItemId);
            if (item == null) throw new Exception("Book Item not found.");

            item.Restore();
            await _bookRepository.RestoreBookItemAsync(item);

            await LogAction("Restore Book Item", $"Restored item with Barcode: {item.Barcode}");
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<string> SaveImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

            var rootPath = AppContext.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.None)[0];
            var filePath = Path.Combine(rootPath, "assets", "covers", fileName);

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/assets/covers/{fileName}";
        }

        private async Task LogAction(string action, string details)
        {
            var auditLog = new AuditLog(
                _currentUserService.UserId,
                _currentUserService.Username,
                action, "Library", "Books", string.Empty,
                $"{details} at {DateTime.Now}");
            await _auditRepository.AddAsync(auditLog);
        }
    }
}